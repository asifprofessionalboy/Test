.scrollable-grid {
    height: 450px;
    overflow: auto;
    border: 1px solid #ccc;
}


/* First (grouped) header row */
.groupHeaderRow th {
    position: sticky;
    top: 0;
    background-color: #dceefb;
    z-index: 3;
    border: 1px solid #ccc;
    text-align: center;
    font-weight: bold;
}

/* Second (built-in GridView header) */
.grid th {
    position: sticky;
    top: 40px; /* Match this to height of custom row */
    z-index: 2;
    background-color: #4A90E2;
    color: white;
    border: 1px solid #ccc;
    text-align: center;
}
