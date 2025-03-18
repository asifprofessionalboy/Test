getting error on these 
var grayCaptured = new Image<Gray, byte>(captured);
var grayStored = new Image<Gray, byte>(stored);

Argument 1: cannot convert from 'System.Drawing.Bitmap' to 'byte[*,*,*]'

and on these 
faceRecognizer.Train(new Image<Gray, byte>[] { grayStored }, new int[] { 1 });

Argument 1: cannot convert from 'Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>[]' to 'Emgu.CV.IInputArrayOfArrays'
