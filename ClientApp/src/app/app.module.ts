import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule, MatButtonModule, MatSelectModule, MatCheckboxModule,
         MatIconModule, MatProgressBarModule, MatSlideToggleModule } from '@angular/material';
import { AppRoutingModule } from './app-routing.module';
import { ChartsModule, ThemeService } from 'ng2-charts';
import { SessionStorageService } from './services/sessionStorage.service';
import { CandidateService } from './services/candidate.service';
import { ChartService } from './services/chart.service';
import { CandidateSignalRService } from './services/signal-r/candidate.signal-r.service';
import { ChartSignalRService } from './services/signal-r/chart.signal-r.service';
import { BoardSignalRService } from './services/signal-r/board.signal-r.service';
import { AppComponent } from './app.component';
import { CandidatesComponent } from './components/candidates/candidates.component';
import { CandidateComponent } from './components/candidate/candidate.component';
import { CandidateAddEditComponent } from './components/candidate-add-edit/candidate-add-edit.component';
import { ChartComponent } from './components/chart/chart.component';
import { BoardComponent } from './components/board/board.component';

@NgModule({
  declarations: [
    AppComponent,
    CandidatesComponent,
    CandidateComponent,
    CandidateAddEditComponent,
    ChartComponent,
    BoardComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatSelectModule,
    MatIconModule,
    MatProgressBarModule,
    MatSlideToggleModule,
    HttpClientModule,
    AppRoutingModule,
    ChartsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule
  ],
  providers: [
    SessionStorageService,
    CandidateService,
    ChartService,
    CandidateSignalRService,
    ChartSignalRService,
    BoardSignalRService,
    ThemeService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
