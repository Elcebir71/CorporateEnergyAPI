window.setupLineChart = function (canvasId, labels, dataValues) {
    var canvas = document.getElementById(canvasId);
    if (!canvas) {
        console.warn("Canvas not found:", canvasId);
        return;
    }

    // Bestaande grafiek vernietigen
    var existingChart = Chart.getChart(canvasId);
    if (existingChart) {
        existingChart.destroy();
    }

    // Container hoogte instellen
    var container = canvas.parentElement;
    if (container) {
        canvas.style.height = '100%';
        canvas.style.width = '100%';
    }

    new Chart(canvas, {
        type: "line",
        data: {
            labels: labels,
            datasets: [{
                label: "Prijs (€/MWh)",
                data: dataValues,
                borderColor: "#66fcf1",
                backgroundColor: "rgba(102, 252, 241, 0.1)",
                tension: 0.3,
                fill: true,
                pointRadius: 0
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                intersect: false,
                mode: "index"
            },
            scales: {
                x: {
                    grid: { color: "#222" },
                    ticks: {
                        color: "#888",
                        maxRotation: 0,
                        maxTicksLimit: 10,
                        autoSkip: true
                    }
                },
                y: {
                    grid: { color: "#222" },
                    ticks: { color: "#888" },
                    beginAtZero: false
                }
            },
            plugins: {
                legend: {
                    labels: { color: "#66fcf1" }
                },
                tooltip: {
                    backgroundColor: "#1a1a1a",
                    titleColor: "#66fcf1",
                    bodyColor: "#fff",
                    borderColor: "#66fcf1",
                    borderWidth: 1
                }
            }
        }
    });
};

window.downloadCsvFromBlazor = function (fileName, data) {
    try {
        var csv = "Timestamp;Price_MWh;Is_Green_Energy;System_Code\n";
        for (var i = 0; i < data.length; i++) {
            csv += data[i].timestamp + ";" + data[i].price_MWh + ";" + data[i].is_Green_Energy + ";" + data[i].system_Code + "\n";
        }
        var blob = new Blob([csv], { type: "text/csv;charset=utf-8;" });
        var url = URL.createObjectURL(blob);
        var link = document.createElement("a");
        link.href = url;
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    } catch (err) {
        console.error("Export failed:", err);
    }
};