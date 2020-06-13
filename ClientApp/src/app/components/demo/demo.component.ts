import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { DemoSignalRService } from 'src/app/services/demo.signal-r.service';
import { HttpClient } from '@angular/common/http';
import { Chart } from '../../models/chart';

@Component({
  selector: 'app-demo-chart',
  templateUrl: './demo.component.html',
  styleUrls: ['./demo.component.css']
})
export class DemoComponent implements OnInit {
  chartUrl = `${environment.functionUrl}/api/chart`;
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
  public chartLabels: string[] = ['Timer trigger based chart data'];
  public chartType = 'bar';
  public chartLegend = true;
  public colors: any[] = [
    { backgroundColor: '#5491DA' },
    { backgroundColor: '#E74C3C' },
    { backgroundColor: '#82E0AA' },
    { backgroundColor: '#E5E7E9' }
  ];

  constructor(public signalRService: DemoSignalRService, private http: HttpClient) { }

  ngOnInit(): void {
    if (!this.signalRService.isConnected) {
      this.startConnection();
    }
  }

  startConnection() {
    this.signalRService.startConnection();
    this.signalRService.addTransferChartDataListener();
    this.startHttpRequest();
  }

  startHttpRequest = () => {
    this.http.get(this.chartUrl)
      .subscribe(results => {
        this.signalRService.data = results as Chart[];
      });
  }

}
