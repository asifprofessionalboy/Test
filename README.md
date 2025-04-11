
i want this to stick on bottom of page 
<footer>
    <div class="text-center d-flex justify-content-center" style="margin-top:93%;">


        <div class="button-container" style="">
            <button class="button21" onclick="redirectToHome()">
                <svg xmlns="http://www.w3.org/2000/svg" width="1em" height="1em" viewBox="0 0 1024 1024" stroke-width="0" fill="currentColor" stroke="currentColor" class="icon">
                    <path d="M946.5 505L560.1 118.8l-25.9-25.9a31.5 31.5 0 0 0-44.4 0L77.5 505a63.9 63.9 0 0 0-18.8 46c.4 35.2 29.7 63.3 64.9 63.3h42.5V940h691.8V614.3h43.4c17.1 0 33.2-6.7 45.3-18.8a63.6 63.6 0 0 0 18.7-45.3c0-17-6.7-33.1-18.8-45.2zM568 868H456V664h112v204zm217.9-325.7V868H632V640c0-22.1-17.9-40-40-40H432c-22.1 0-40 17.9-40 40v228H238.1V542.3h-96l370-369.7 23.1 23.1L882 542.3h-96.1z"></path>
                </svg>

            </button>
            <button class="button21" onclick="redirectToIframePage()">
                <svg class="svgIcon" viewBox="0 0 384 512">
                    <path d="M64 32C46.3 32 32 46.3 32 64V448c0 17.7 14.3 32 32 32H320c17.7 0 32-14.3 32-32V160L240 32H64zM272 48l80 80H272V48zM128 232c0-8.8 7.2-16 16-16h112c8.8 0 16 7.2 16 16s-7.2 16-16 16H144c-8.8 0-16-7.2-16-16zm0 64c0-8.8 7.2-16 16-16h112c8.8 0 16 7.2 16 16s-7.2 16-16 16H144c-8.8 0-16-7.2-16-16zm0 64c0-8.8 7.2-16 16-16h80c8.8 0 16 7.2 16 16s-7.2 16-16 16H144c-8.8 0-16-7.2-16-16zm208-16c6.2 6.2 6.2 16.4 0 22.6l-48 48c-6.2 6.2-16.4 6.2-22.6 0l-24-24c-6.2-6.2-6.2-16.4 0-22.6s16.4-6.2 22.6 0l12 12 36.4-36.4c6.2-6.2 16.4-6.2 22.6 0z" />
                </svg>
            </button>
            <button class="button21" onclick="redirectToHome()">
                <svg xmlns="http://www.w3.org/2000/svg" width="1em" height="1em" viewBox="0 0 24 24" stroke-width="0" fill="currentColor" stroke="currentColor" class="icon">
                    <path d="M12 2.5a5.5 5.5 0 0 1 3.096 10.047 9.005 9.005 0 0 1 5.9 8.181.75.75 0 1 1-1.499.044 7.5 7.5 0 0 0-14.993 0 .75.75 0 0 1-1.5-.045 9.005 9.005 0 0 1 5.9-8.18A5.5 5.5 0 0 1 12 2.5ZM8 8a4 4 0 1 0 8 0 4 4 0 0 0-8 0Z"></path>
                </svg>
            </button>

            @* <button class="button21" onclick="redirectToChangePassword()">
            <svg xmlns="http://www.w3.org/2000/svg" width="1em" height="1em" stroke-linejoin="round" stroke-linecap="round" viewBox="0 0 24 24" stroke-width="2" fill="none" stroke="currentColor" class="icon">
            <circle r="1" cy="21" cx="9"></circle>
            <circle r="1" cy="21" cx="20"></circle>
            <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
            </svg>
            </button> *@
        </div>
    </div>
</footer>

.button-container {
    display: flex;
    position:;
    background-color: rgb(57 62 243);
    width: 100%;
    height: 40px;
    align-items: center;
    justify-content: space-around;
    border-radius: 10px;
    box-shadow: rgba(0, 0, 0, 0.35) 0px 5px 15px, rgb(73 85 245 / 50%) 5px 10px 15px;
}

.button21 {
    outline: 0 !important;
    border: 0 !important;
    width: 40px;
    height: 40px;
    border-radius: 50%;
    background-color: transparent;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #fff;
    transition: all ease-in-out 0.3s;
    cursor: pointer;
}

    .button21:hover {
        transform: translateY(-3px);
    }

.icon {
    font-size: 20px;
}
