import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ChartSignalRService } from 'src/app/services/chart.signal-r.service';
import { HttpClient } from '@angular/common/http';
import { Chart } from '../../models/chart';

@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css']
})
export class ChartComponent implements OnInit {
  chartUrl = `${environment.baseUrl}/api/chart`;
  chartOptions: any = {
    scaleShowVerticalLines: true,
    responsive: true,
    scales: {
      yAxes: [{
        ticks: {
          beginAtZero: true
        }
      }]
    }
  };
  public chartLabels: string[] = ['Change feed listener based chart data'];
  public chartType = 'bar';
  public chartLegend = true;
  public colors: any[] = [
    { backgroundColor: '#5491DA' },
    { backgroundColor: '#E74C3C' },
    { backgroundColor: '#82E0AA' },
    { backgroundColor: '#E5E7E9' }
  ];

  constructor(public signalRService: ChartSignalRService, private http: HttpClient) { }

  ngOnInit(): void {
    if (!this.signalRService.isConnected) {
      this.startConnection();
    }
  }

  startConnection() {
    this.signalRService.startConnection();
    this.signalRService.addTransferChartDataListener();
    this.signalRService.addBroadcastChartDataListener();
    this.startHttpRequest();
  }

  startHttpRequest = () => {
    this.http.get(this.chartUrl)
      .subscribe(results => {
        console.log(results);
        this.signalRService.data = results as Chart[];
      });
  }

  public chartClicked = (event: any) => {
    console.log(event);
    // this.signalRService.broadcastChartData();
  }

}
