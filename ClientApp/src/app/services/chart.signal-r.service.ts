import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { environment } from 'src/environments/environment';
import { Chart } from '../models/chart';
import { HubConnectionState } from '@aspnet/signalr';

@Injectable()
export class ChartSignalRService {
  private chartUrl: string;
  private hubConnection: signalR.HubConnection;
  public data: Chart[];
  public broadcastedData: Chart[];

  public get isConnected(): boolean {
    return this.hubConnection
    && this.hubConnection.state === HubConnectionState.Connected;
  }

  constructor() {
    this.chartUrl = `${environment.baseUrl}/chart`;
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

  public addBroadcastChartDataListener = () => {
    this.hubConnection.on('broadcastChartData', (data) => {
      this.broadcastedData = data;
    });
  }

  public broadcastChartData = () => {
    this.hubConnection.invoke('broadcastChartData', this.data)
    .catch(err => console.error(err));
  }

}
