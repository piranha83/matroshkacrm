import { Injectable, NgModule } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class DictionaryService {
    dictionary: Array<Observable<Object[]>> = [];
    url: string;

    constructor(private http: HttpClient) {
        this.url = 'http://api.open-notify.org/astros.json';
    }

    get(key: string): Observable<Object[]> {
        if (!this.dictionary[key]) {
            this.dictionary[key] = this.http.get(this.url).pipe(map(responce => {
                let models = responce[key];
                return models.map(function (model: any) {
                    return {
                        key: model.name,
                        value: model.name,
                        data: model
                    };
                });
            }));
        }
        return this.dictionary[key];
    }
}
