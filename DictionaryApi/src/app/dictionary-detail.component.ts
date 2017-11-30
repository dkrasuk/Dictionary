import 'rxjs/add/operator/switchMap';
import { Component, OnInit} from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Location }                 from '@angular/common';
import { Dictionary}                from './dictionary';
import { DictionaryService}         from './dictionary.service';
import { Item}                       from './item';

@Component({
    selector: 'dictionary-detail',
    templateUrl: './dictionary-detail.component.html',
    styleUrls: [ './dictionary-detail.component.css' ]
})

export class DictionaryDetailComponent implements OnInit {
    dictionary: Dictionary;
    initialDictionaryName: string;
    initialDictionaryVersion: string;
    newItemValue: string;

    constructor(
        private dictionaryService: DictionaryService,
        private route: ActivatedRoute,
        private location: Location
    ) {}

    ngOnInit(): void {
        this.route.params.subscribe(params => {
            this.initialDictionaryName = params['dictionaryName'];
            this.initialDictionaryVersion = params['version'];
        });

        this.route.paramMap
            .switchMap( (params: ParamMap) => this.dictionaryService.getDictionary(params.get('dictionaryName'), params.get('version')))
            .subscribe(dictionary => this.dictionary = dictionary);
    }

    isJsonString(str: string) {
        try {
            JSON.parse(str);
        } catch (e) {
            return false;
        }
        return true;
    }

    createItem(dictionary: Dictionary): void {
        this.newItemValue = this.newItemValue.trim();

        if (!this.newItemValue) {
            alert('New item value can\'t be empty');
            return;
        }

        if (!this.isJsonString(this.newItemValue)) {
            alert('New item value should be valid JSON string: \n' + this.newItemValue);
            return;
        }

        let parsedItemValue = JSON.parse(this.newItemValue);

        if (typeof parsedItemValue.id === 'undefined') {
            alert('Item JSON value should contain mandatory field \'id\'');
            return;
        }

        this.fillNewItemPattern();

        this.dictionaryService.createItem(dictionary.Name, dictionary.Version, parsedItemValue)
            .then( () => {
                let item = new Item();
                item.ValueId = parsedItemValue.id;

                item.ValueString = this.newItemValue;

                dictionary.Items.push(item);

                this.newItemValue = '';
            });
    }

    save(): void {
        this.dictionaryService.update(this.initialDictionaryName, this.initialDictionaryVersion, this.dictionary)
            .then(() => null);
    }

    goBack(): void {
        this.location.back();
    }

    deleteItem(itemId: string): void {
        if (!confirm('Remove dictionary item "' + itemId + '"?')) {
            return;
        }
        itemId = itemId.trim();

        if (!itemId) { return }

        this.dictionaryService.deleteItem(this.dictionary.Name, this.dictionary.Version, itemId)
            .then(() => {
                this.dictionary.Items = this.dictionary.Items.filter((item) => item.ValueId !== itemId);
            });
    }

    private getNextDictionaryItemId(): number {
        let sortedList = this.dictionary.Items.map((a) => a.ValueId).sort(this.comparator);
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

    fillNewItemPattern(): void {
        if (typeof this.newItemValue === 'undefined' || this.newItemValue.length === 0) {
            this.newItemValue = '{\r\n\t"id": "' + this.getNextDictionaryItemId() + '"\r\n}';
        } else {
            if (this.isJsonString(this.newItemValue)) {
                this.newItemValue = JSON.stringify(JSON.parse(this.newItemValue), null, '\t');
            }
        }
    }

    dictionaryIsEmpty(): boolean {
        return typeof this.dictionary === 'undefined';
    }
}

