import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { environment } from 'src/environments/environment';
import { Chart } from '../models/chart';
import { HubConnectionState } from '@aspnet/signalr';

@Injectable()
export class DemoSignalRService {
  private chartUrl: string;
  private hubConnection: signalR.HubConnection;
  public data: Chart[];

  public get isConnected(): boolean {
    return this.hubConnection
    && this.hubConnection.state === HubConnectionState.Connected;
  }

  constructor() {
    this.chartUrl = `${environment.functionUrl}/api`;
  }

  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.chartUrl)
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
      })
      .catch(err => console.log('Error while starting connection: ' + err));
  }

  public addTransferChartDataListener = () => {
    this.hubConnection.on('transferChartData', (data) => {
      this.data = data;
      console.log(data);
    });
  }

}
