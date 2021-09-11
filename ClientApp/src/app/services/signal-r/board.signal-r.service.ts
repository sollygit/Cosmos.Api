import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { environment } from 'src/environments/environment';
import { Chart } from '../../models/chart';
import { HubConnectionState } from '@aspnet/signalr';

@Injectable()
export class BoardSignalRService {
  private boardUrl: string;
  private hubConnection: signalR.HubConnection;
  public boardData: Chart[];

  public get isConnected(): boolean {
    return this.hubConnection
    && this.hubConnection.state === HubConnectionState.Connected;
  }

  constructor() {
    this.boardUrl = `${environment.baseUrl}/board`;
  }

  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.boardUrl)
      .build();
    this.hubConnection
      .start()
      .then(() => {
        console.log(`Connection started on ${this.boardUrl}`);
      })
      .catch(err => console.log(`Error while starting connection: ${err}`));
  }

  public addTimerTriggerBoardDataListener = () => {
    this.hubConnection.on('timerTriggerBoardData', (boardData: Chart[]) => {
      this.boardData = boardData;
      console.log(boardData);
    });
  }

}
