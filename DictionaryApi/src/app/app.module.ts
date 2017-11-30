import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule }   from '@angular/forms';
import { HttpModule }    from '@angular/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent }         from './app.component';
import { DictionariesComponent }      from './dictionaries.component';
import { DictionaryDetailComponent }  from './dictionary-detail.component';
import { DictionaryService }          from './dictionary.service';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  imports: [
      BrowserModule,
      NgbModule.forRoot(),
      FormsModule,
      HttpModule,
      AppRoutingModule
  ],
  declarations: [
    AppComponent,
    DictionaryDetailComponent,
    DictionariesComponent
  ],
  providers: [ DictionaryService ],
  bootstrap: [ AppComponent ]
})
export class AppModule { }
