<button class="button">
    <svg class="svgIcon" viewBox="0 0 384 512">
        <path d="M64 32C46.3 32 32 46.3 32 64V448c0 17.7 14.3 32 32 32H320c17.7 0 32-14.3 32-32V160L240 32H64zM272 48l80 80H272V48zM96 224h192c8.8 0 16-7.2 16-16s-7.2-16-16-16H96c-8.8 0-16 7.2-16 16s7.2 16 16 16zm0 64h192c8.8 0 16-7.2 16-16s-7.2-16-16-16H96c-8.8 0-16 7.2-16 16s7.2 16 16 16zm0 64h128c8.8 0 16-7.2 16-16s-7.2-16-16-16H96c-8.8 0-16 7.2-16 16s7.2 16 16 16z"/>
    </svg>
</button>

.svgIcon {
    width: 18px; /* Adjust for better visibility */
    transition-duration: 0.3s;
}

<svg class="svgIcon" viewBox="0 0 384 512">
    <path d="M64 32C46.3 32 32 46.3 32 64V448c0 17.7 14.3 32 32 32H320c17.7 0 32-14.3 32-32V160L240 32H64zM272 48l80 80H272V48zM128 232c0-8.8 7.2-16 16-16h112c8.8 0 16 7.2 16 16s-7.2 16-16 16H144c-8.8 0-16-7.2-16-16zm0 64c0-8.8 7.2-16 16-16h112c8.8 0 16 7.2 16 16s-7.2 16-16 16H144c-8.8 0-16-7.2-16-16zm0 64c0-8.8 7.2-16 16-16h80c8.8 0 16 7.2 16 16s-7.2 16-16 16H144c-8.8 0-16-7.2-16-16zm208-16c6.2 6.2 6.2 16.4 0 22.6l-48 48c-6.2 6.2-16.4 6.2-22.6 0l-24-24c-6.2-6.2-6.2-16.4 0-22.6s16.4-6.2 22.6 0l12 12 36.4-36.4c6.2-6.2 16.4-6.2 22.6 0z"/>
</svg>



this is my button i want for report please provide report icon for this  
<button class="button">
     <svg class="svgIcon" viewBox="0 0 384 512">
         <path d="M214.6 41.4c-12.5-12.5-32.8-12.5-45.3 0l-160 160c-12.5 12.5-12.5 32.8 0 45.3s32.8 12.5 45.3 0L160 141.2V448c0 17.7 14.3 32 32 32s32-14.3 32-32V141.2L329.4 246.6c12.5 12.5 32.8 12.5 45.3 0s12.5-32.8 0-45.3l-160-160z"></path>
     </svg>
 </button>

this is my css

 .button {
     width: 50px;
     height: 50px;
     border-radius: 50%;
     background-color: rgb(20, 20, 20);
     border: none;
     font-weight: 600;
     display: flex;
     align-items: center;
     justify-content: center;
     box-shadow: 0px 0px 0px 4px rgba(180, 160, 255, 0.253);
     cursor: pointer;
     transition-duration: 0.3s;
     overflow: hidden;
     position: relative;
 }

 .svgIcon {
     width: 12px;
     transition-duration: 0.3s;
 }

     .svgIcon path {
         fill: white;
     }

 .button:hover {
     width: 140px;
     border-radius: 50px;
     transition-duration: 0.3s;
     background-color: rgb(181, 160, 255);
     align-items: center;
 }

     .button:hover .svgIcon {
         /* width: 20px; */
         transition-duration: 0.3s;
         transform: translateY(-200%);
     }

 .button::before {
     position: absolute;
     bottom: -20px;
     content: "Back to Top";
     color: white;
     /* transition-duration: .3s; */
     font-size: 0px;
 }

 .button:hover::before {
     font-size: 13px;
     opacity: 1;
     bottom: unset;
     /* transform: translateY(-30px); */
     transition-duration: 0.3s;
 }
