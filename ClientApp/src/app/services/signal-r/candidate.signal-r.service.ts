import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { HubConnectionState } from '@aspnet/signalr';
import { environment } from 'src/environments/environment';
import { Candidate } from '../../models/candidate';

@Injectable()
export class CandidateSignalRService {
  private candidateUrl: string;
  private hubConnection: signalR.HubConnection;
  public data: Candidate[];

  public get isConnected(): boolean {
    return this.hubConnection
    && this.hubConnection.state === HubConnectionState.Connected;
  }

  constructor() {
    this.candidateUrl = `${environment.baseUrl}/candidate`;
  }

  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Debug)
      .withUrl(this.candidateUrl)
      .build();
    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
      })
      .catch(err => console.log('Error while starting connection: ' + err));
  }

  public addSendCandidatesListener = (isActive: boolean) => {
    this.hubConnection.on('sendCandidates', (data: Candidate[]) => {
      this.data = data.filter(o => o.isActive === isActive);
      console.log(data);
    });
  }

}
