import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { BoardSignalRService } from 'src/app/services/signal-r/board.signal-r.service';
import { HttpClient } from '@angular/common/http';
import { Chart } from '../../models/chart';
import { ChartService } from 'src/app/services/chart.service';

@Component({
  selector: 'app-board',
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.css']
})
export class BoardComponent implements OnInit {
  boardUrl = `${environment.baseUrl}/api/board`;
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
  public chartLabels: string[] = ['Timer trigger based board data'];
  public chartType = 'bar';
  public chartLegend = true;
  public colors: any[] = [
    { backgroundColor: '#5491DA' },
    { backgroundColor: '#E74C3C' },
    { backgroundColor: '#82E0AA' },
    { backgroundColor: '#E5E7E9' }
  ];

  constructor(
    private chartService: ChartService,
    private signalRService: BoardSignalRService,
    private http: HttpClient) { }

  ngOnInit(): void {
    if (!this.signalRService.isConnected) {
      this.startConnection();
    }
  }

  startConnection() {
    this.signalRService.startConnection();
    this.signalRService.addTimerTriggerBoardDataListener();
    this.startHttpRequest();
  }

  startHttpRequest = () => {
    this.http.get(this.boardUrl)
      .subscribe(res => {
        console.log(res);
        this.getBoardData();
      });
  }

  getBoardData() {
    this.chartService.getBoardData()
      .subscribe(boardData => {
        this.boardData = boardData;
      });
  }

  set boardData(value: Chart[]) {
    this.signalRService.boardData = value;
  }
  get boardData(): Chart[] {
    return this.signalRService.boardData;
  }

}
