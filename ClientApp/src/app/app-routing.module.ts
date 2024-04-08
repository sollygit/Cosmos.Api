import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CandidatesComponent } from './components/candidates/candidates.component';
import { CandidateComponent } from './components/candidate/candidate.component';
import { CandidateAddEditComponent } from './components/candidate-add-edit/candidate-add-edit.component';
import { ChartComponent } from './components/chart/chart.component';
import { BoardComponent } from './components/board/board.component';

const routes: Routes = [
  { path: '', component: CandidatesComponent, pathMatch: 'full' },
  { path: 'candidate/:id', component: CandidateComponent },
  { path: 'add', component: CandidateAddEditComponent },
  { path: 'candidate/edit/:id', component: CandidateAddEditComponent },
  { path: 'chart', component: ChartComponent },
  { path: 'board', component: BoardComponent },
  { path: '**', redirectTo: '/' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
