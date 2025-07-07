content of facenet.py

# This script is mostly based on the openpose preprocessor script of
# the sd-webui-controlnet project by Mikubill.
# https://github.com/Mikubill/sd-webui-controlnet/blob/main/annotator/openpose/face.py

import numpy as np
import onnxruntime as ort
import cv2
from PIL import Image
import pathlib
from typing import Tuple, Union, List
from tqdm import tqdm


def smart_resize(image: np.ndarray, shape: Tuple[int, int]) -> np.ndarray:
    """
    Resize an image to a target shape while preserving aspect ratio.

    Parameters
    ----------
    image : np.ndarray
        The input image.
    shape : Tuple[int, int]
        The target shape (height, width).

    Returns
    -------
    np.ndarray
        The resized image
    """

    Ht, Wt = shape
    if image.ndim == 2:
        Ho, Wo = image.shape
        Co = 1
    else:
        Ho, Wo, Co = image.shape
    if Co == 3 or Co == 1:
        k = float(Ht + Wt) / float(Ho + Wo)
        return cv2.resize(
            image,
            (int(Wt), int(Ht)),
            interpolation=cv2.INTER_AREA if k < 1 else cv2.INTER_LANCZOS4,
        )
    else:
        return np.stack(
            [smart_resize(image[:, :, i], shape) for i in range(Co)], axis=2
        )


class FaceLandmarkDetector:
    """
    The OpenPose face landmark detector model using ONNXRuntime.

    Parameters
    ----------
    face_model_path : str
        The path to the ONNX model file.
    """

    def __init__(self, face_model_path: pathlib.Path) -> None:
        """
        Initialize the OpenPose face landmark detector model.

        Parameters
        ----------
        face_model_path : pathlib.Path
            The path to the ONNX model file.
        """

        # Initialize ONNX runtime session
        self.session = ort.InferenceSession(
            face_model_path, providers=["CPUExecutionProvider"]
        )
        self.input_name = self.session.get_inputs()[0].name

    def _inference(self, face_img: np.ndarray) -> np.ndarray:
        """
        Run the OpenPose face landmark detector model on an image.

        Parameters
        ----------
        face_img : np.ndarray
            The input image.

        Returns
        -------
        np.ndarray
            The detected keypoints.
        """

        # face_img should be a numpy array: H x W x C (likely RGB or BGR)
        H, W, C = face_img.shape

        # Preprocessing
        w_size = 384  # ONNX is exported for this size
        # Resize input image
        resized_img = cv2.resize(
            face_img, (w_size, w_size), interpolation=cv2.INTER_LINEAR
        )

        # Normalize: /256.0 - 0.5 (mimicking original code)
        x_data = resized_img.astype(np.float32) / 256.0 - 0.5

        # Convert to channel-first format: (C, H, W)
        x_data = np.transpose(x_data, (2, 0, 1))

        # Add batch dimension: (1, C, H, W)
        x_data = np.expand_dims(x_data, axis=0)

        # Run inference
        outputs = self.session.run(None, {self.input_name: x_data})

        # Assuming the model's last output corresponds to the heatmaps
        # and is shaped like (1, num_parts, h_out, w_out)
        heatmaps_original = outputs[-1]

        # Remove batch dimension: (num_parts, h_out, w_out)
        heatmaps_original = np.squeeze(heatmaps_original, axis=0)

        # Resize the heatmaps back to the original image size
        num_parts = heatmaps_original.shape[0]
        heatmaps = np.zeros((num_parts, H, W), dtype=np.float32)
        for i in range(num_parts):
            heatmaps[i] = cv2.resize(
                heatmaps_original[i], (W, H), interpolation=cv2.INTER_LINEAR
            )

        peaks = self.compute_peaks_from_heatmaps(heatmaps)

        return peaks

    def __call__(
        self,
        face_img: Union[np.ndarray, List[np.ndarray], Image.Image, List[Image.Image]],
    ) -> List[np.ndarray]:
        """
        Run the OpenPose face landmark detector model on an image.

        Parameters
        ----------
        face_img : Union[np.ndarray, Image.Image, List[Image.Image]]
            The input image or a list of input images.

        Returns
        -------
        List[np.ndarray]
            The detected keypoints.
        """

        if isinstance(face_img, Image.Image):
            image_list = [np.array(face_img)]
        elif isinstance(face_img, list):
            if isinstance(face_img[0], Image.Image):
                image_list = [np.array(img) for img in face_img]
        elif isinstance(face_img, np.ndarray):
            if face_img.ndim == 4:
                image_list = [img for img in face_img]

        results = []

        for image in tqdm(image_list):
            keypoints = self._inference(image)
            results.append(keypoints)

        return results

    def compute_peaks_from_heatmaps(self, heatmaps: np.ndarray) -> np.ndarray:
        """
        Compute the peaks from the heatmaps.

        Parameters
        ----------
        heatmaps : np.ndarray
            The heatmaps.

        Returns
        -------
        np.ndarray
            The peaks, which are keypoints.
        """

        all_peaks = []
        for part in range(heatmaps.shape[0]):
            map_ori = heatmaps[part].copy()
            binary = np.ascontiguousarray(map_ori > 0.05, dtype=np.uint8)

            if np.sum(binary) == 0:
                all_peaks.append([-1, -1])
                continue

            positions = np.where(binary > 0.5)
            intensities = map_ori[positions]
            mi = np.argmax(intensities)
            y, x = positions[0][mi], positions[1][mi]
            all_peaks.append([x, y])

        return np.array(all_peaks)
