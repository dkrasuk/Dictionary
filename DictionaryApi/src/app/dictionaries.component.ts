import { Component, OnInit } from '@angular/core';
import { Router }            from '@angular/router';
import { Dictionary }        from './dictionary';
import { DictionaryService } from './dictionary.service';

@Component({
    selector: 'dictionaries-app',
    templateUrl: './dictionaries.component.html',
    styleUrls: ['./dictionaries.component.css']
})

export class DictionariesComponent implements OnInit {
    dictionaries: Dictionary[];
    selectedDictionary: Dictionary;

    constructor(
        private dictionaryService: DictionaryService,
        private router: Router) {
    }

    getDictionaries(): void {
        this.dictionaryService
            .getDictionaries()
            .then(dictionaries => this.dictionaries = dictionaries);
    }

    dictionariesIsEmpty(): boolean {
        return typeof this.dictionaries === 'undefined';
    }

    add(dictionaryName: string): void {
        dictionaryName = dictionaryName.trim();

        if (!dictionaryName) { return }

        let dictionary = new Dictionary();
        dictionary.Name = dictionaryName;
        dictionary.Version = '' + this.getNextDictionaryVersion(this.dictionaries.filter((d) => d.Name === dictionaryName));

        this.dictionaryService.create(dictionary)
            .then(dict => {
                this.dictionaries.push(dict);
                this.selectedDictionary = dict;
                this.dictionaries.forEach((d) => d.Selected = false);
                this.selectedDictionary.Selected = true;
            });
    }

    private getNextDictionaryVersion(dictionaries: Dictionary[]): number {
        if (dictionaries.length === 0) {
            return 1;
        }

        let sortedList = dictionaries.map((a) => a.Version).sort(this.comparator);
        let lastArrayNumber = Number(sortedList[sortedList.length - 1]);

        return isNaN(lastArrayNumber) ? 1 : lastArrayNumber + 1;
    }

    private comparator(a: string, b: string): number {
        let a_num = Number(a),
            b_num = Number(b);

        if (isNaN(a_num)) {
            return -9999;
        }
        if (isNaN(b_num)) {
            return 0.1;
        }
        return a_num - b_num;
    }

    delete(dictionaryName: string, version: string): void {
        if (!confirm('Remove dictionary "' + dictionaryName + '"?')) {
            return;
        }
        dictionaryName = dictionaryName.trim();
        version = version.trim();

        if (!dictionaryName) { return }
        if (!version) { return }

        this.dictionaryService.delete(dictionaryName, version)
            .then(() => {
                this.dictionaries = this.dictionaries.filter(d => d.Name + d.Version !== dictionaryName + version);

                if (typeof this.selectedDictionary === 'undefined') { return; }

                if (this.selectedDictionary.Name === dictionaryName && this.selectedDictionary.Version === version) {
                    this.selectedDictionary = null;
                }
            });
    }

    ngOnInit(): void {
        this.getDictionaries();
    }

    onSelect(dictionary: Dictionary): void {
        this.selectedDictionary = dictionary;
        this.dictionaries.forEach((d) => d.Selected = false);
        dictionary.Selected = true;
    }

    gotoDetail(): void {
        this.router.navigate(['/detail', this.selectedDictionary.Name, this.selectedDictionary.Version]);
    }
}
