function generateLineChart(Mul, Seconds, Name, RocketPos, PlayerPos) {
    $("#chart").empty();
    var options = {
        series: [{
            data: Mul
        }],
        chart: {
            height: 350,
            type: 'line',
            id: 'areachart-2'
        },
        annotations: {
            points: [{
                x: PlayerPos.mul,
                y: PlayerPos.sec,
                marker: {
                    size: 8,
                    fillColor: '#fff',
                    strokeColor: 'red',
                    radius: 2,
                    cssClass: 'apexcharts-custom-class'
                },
                label: {
                    borderColor: '#FF4560',
                    offsetY: 0,
                    style: {
                        color: '#fff',
                        background: '#FF4560',
                    },

                    text: Name,
                }
            }, {
                x: RocketPos.mul,
                y: RocketPos.sec,
                marker: {
                    size: 0
                },
                image: {
                    path: '../Images/jetpackman.png',
                     width: 70,
                    height: 70
                }
            }]
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            show: true,
            curve: 'smooth',
        },
        grid: {
            padding: {
                right: 30,
                left: 20
            }
        },
        title: {
            text: 'Crash',
            align: 'left'
        },
        labels: Seconds,
        xaxis: {
            type: 'line',
        },
    };
    //follow the same tag id for your visualization
    var chart = new ApexCharts(document.querySelector("#chart"), options);
    chart.render();
}