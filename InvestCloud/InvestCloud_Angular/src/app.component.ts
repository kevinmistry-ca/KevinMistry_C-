import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CategoryListComponent } from './components/category-list/category-list.component';
import { AppletListComponent } from './components/applet-list/applet-list.component';
import { SearchComponent } from './components/search/search.component';

interface Applet {
  name: string;
  categories: string[];
}

interface Library {
  categories: string[];
  applets: Applet[];
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    CategoryListComponent,
    AppletListComponent,
    SearchComponent,
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  lib: Library = {
    categories: ['Performance', 'Investments', 'Operations'],
    applets: [
      { name: 'Performance Snapshot', categories: ['Performance'] },
      { name: 'Commitment Widget', categories: ['Investments'] },
      { name: 'CMS', categories: ['Investments', 'Performance'] },
    ],
  };

  selectedCategory: string | null = null;
  searchQuery: string = '';
  debounceTimer: any; // This will hold the timeout ID

  get filteredCategories() {
    if (!this.searchQuery) {
      return this.lib.categories.map((category) => ({
        name: category,
        count: this.lib.applets.filter(
          (applet) =>
            applet.categories.includes(category) &&
            (!this.searchQuery ||
              applet.name
                .toLowerCase()
                .includes(this.searchQuery.toLowerCase()))
        ).length,
      }));
    }
    return this.searchFilteredCategories;
  }

  get searchFilteredCategories() {
    const filteredApplets = this.filteredApplets;
    let libCategories = this.lib.categories.filter((category) =>
      filteredApplets.some((applet) => applet.categories?.includes(category))
    );

    return libCategories.map((category) => ({
      name: category,
      count: this.lib.applets.filter(
        (applet) =>
          applet.categories.includes(category) &&
          (!this.searchQuery ||
            applet.name.toLowerCase().includes(this.searchQuery.toLowerCase()))
      ).length,
    }));
  }

  get filteredApplets() {
    return this.lib.applets.filter(
      (applet) =>
        (!this.searchQuery ||
          applet.name.toLowerCase().includes(this.searchQuery.toLowerCase())) &&
        (!this.selectedCategory ||
          applet.categories.includes(this.selectedCategory))
    );
  }

  selectCategory(category: string) {
    this.selectedCategory = category;
  }

  updateSearch(query: string) {
    // this.searchQuery = query;
    // this.selectedCategory = null;
    clearTimeout(this.debounceTimer);
    this.debounceTimer = setTimeout(() => {
      this.searchQuery = query;
      this.selectedCategory = null;
    }, 300); // 300ms delay
  }

  // Method to add a large dataset of categories and applets to the library
  addBigData(ncategs: number, napplets: number) {
    for (let i = 0; i < ncategs; i++) {
      this.lib.categories.push('Sample Category ' + i);
    }

    const n = this.lib.categories.length;

    for (let i = 0; i < napplets; i++) {
      const applet: Applet = { name: 'CMS' + i, categories: [] };
      for (let j = 0; j < Math.floor(Math.random() * 10); ++j) {
        const idx = Math.floor(Math.random() * n) % n;
        applet.categories.push(this.lib.categories[idx]);
      }
      this.lib.applets.push(applet);
    }
  }
}
