import { Component }          from '@angular/core';

@Component({
  selector: 'my-app',
  template: `
    <div class="navbar navbar-inverse navbar-fixed-top">
        <div class="container">
            <div class="navbar-header">
                <a class="navbar-brand" href="#">Dictionary Web</a>
            </div>
        </div>
    </div>
    <router-outlet></router-outlet>
  `,
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Dictionaries Web';
}
