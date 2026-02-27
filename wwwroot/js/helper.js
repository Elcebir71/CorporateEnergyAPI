window.downloadCsvFromBlazor = (fileName, data) => {
    let csvContent = "data:text/csv;charset=utf-8,ID;Sensor;Waarde (kW);Tijdstip\n";

    data.forEach(item => {
        // Bouw de CSV rijen op
        let row = `${item.id};${item.sensorName};${item.value.toString().replace('.', ',')};${item.timestamp}`;
        csvContent += row + "\n";
    });

    const encodedUri = encodeURI(csvContent);
    const link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", fileName);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};