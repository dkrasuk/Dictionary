import { NgModule }             from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DictionariesComponent }      from './dictionaries.component';
import { DictionaryDetailComponent }  from './dictionary-detail.component';

const routes: Routes = [
  { path: '', redirectTo: '/dictionaries', pathMatch: 'full' },
  { path: 'detail/:dictionaryName/:version', component: DictionaryDetailComponent },
  { path: 'dictionaries',     component: DictionariesComponent }
];

@NgModule({
  imports: [ RouterModule.forRoot(routes) ],
  exports: [ RouterModule ]
})
export class AppRoutingModule {}
