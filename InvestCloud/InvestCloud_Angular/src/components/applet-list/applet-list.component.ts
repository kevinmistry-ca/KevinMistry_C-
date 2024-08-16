import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

interface Applet {
  name: string;
  categories: string[];
}

@Component({
  selector: 'app-applet-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './applet-list.component.html',
  styleUrls: ['./applet-list.component.css'],
})
export class AppletListComponent {
  @Input() applets: Applet[] = [];
}
