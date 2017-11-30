import { Injectable }    from '@angular/core';
import { Headers, Http } from '@angular/http';

import 'rxjs/add/operator/toPromise';

import { Dictionary } from './dictionary';
import {Item} from './item';

@Injectable()
export class DictionaryService {

    private headers = new Headers({'Content-Type': 'application/json'});
    private dictionariesUrl = '';

    constructor(private http: Http) {
        var getUrl = window.location;
        this.dictionariesUrl = getUrl .protocol + '//' + getUrl.host + '/dictionary/v1';
        //this.dictionariesUrl = 'http://localhost:8946/dictionary/v1';
    }

    getDictionaries(): Promise<Dictionary[]> {
        return this.http.get(`${this.dictionariesUrl}?version=1`)
            .toPromise()
            .then(response => {
                let result = response.json() as Dictionary[];
                return result.sort((a, b) => {
                    let textA = a.Name.toUpperCase();
                    let textB = b.Name.toUpperCase();
                    return (textA < textB) ? -1 : (textA > textB) ? 1 : 0;
                } );
            })
            .catch(this.handleError);
    }


    getDictionary(dictionaryName: string, version: string): Promise<Dictionary> {
        const url = `${this.dictionariesUrl}/${dictionaryName}/${version}`;

        return this.http.get(url)
            .toPromise()
            .then(response => {
                let result = response.json() as Dictionary;

                result.Items = result.Items.sort(this.compare);

                return result;
            })
            .catch(this.handleError);
    }

    delete(dictionaryName: string, version = '1'): Promise<void> {
        const url = `${this.dictionariesUrl}/${dictionaryName}/${version}`;
        return this.http.delete(url, {headers: this.headers})
            .toPromise()
            .then(() => null)
            .catch(this.handleError);
    }

    create(dictionary: Dictionary): Promise<Dictionary> {
        return this.http
            .post(this.dictionariesUrl, JSON.stringify(dictionary), {headers: this.headers})
            .toPromise()
            .then(res => res.json() as Dictionary)
            .catch(this.handleError);
    }

    update(dictionaryName: string, version: string, dictionary: Dictionary): Promise<Dictionary> {
        const url = `${this.dictionariesUrl}/${dictionaryName}/${version}`;
        return this.http
            .put(url, JSON.stringify(dictionary), {headers: this.headers})
            .toPromise()
            .then(() => dictionary)
            .catch(this.handleError);
    }

    deleteItem(dictionaryName: string, version: string, itemId: string): Promise<void> {
        const url = `${this.dictionariesUrl}/${dictionaryName}/${version}/items/${itemId}`;

        return this.http
            .delete(url, {headers: this.headers})
            .toPromise()
            .then(() => null)
            .catch(this.handleError);
    }

    createItem(dictionaryName: string, version: string, itemBody: object): Promise<void> {
        const url = `${this.dictionariesUrl}/${dictionaryName}/${version}/items`;

        return this.http
            .post(url, JSON.stringify(itemBody), {headers: this.headers})
            .toPromise()
            .then(() => null)
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        console.error('An error occurred', error);
        return Promise.reject(error.message || error);
    }

    private compare(a: Item, b: Item): number {
        let a_num = Number(a.ValueId),
            b_num = Number(b.ValueId);

        if (isNaN(a_num)) {
            return -9999;
        }
        if (isNaN(b_num)) {
            return 0.1;
        }
        return a_num - b_num;
    }
}

