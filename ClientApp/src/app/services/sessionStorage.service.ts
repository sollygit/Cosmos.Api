import { Injectable } from '@angular/core';
import { DBkeys } from './db-keys';

@Injectable({
    providedIn: 'root'
})
export class SessionStorageService {

    public set(key: DBkeys, data: any) {
        sessionStorage.setItem(key as string, JSON.stringify(data));
    }

    public exists(key: DBkeys): boolean {
        return this.get(key) != null;
    }

    public get<T>(key: DBkeys, isDateType = false): T {
        let data = this.JSonTryParse(sessionStorage.getItem(key as string));
        if (data != null) {
            if (isDateType) {
                data = new Date(data);
            }
            return data as T;
        }
        return null;
    }

    private JSonTryParse(value: string) {
        try {
            return JSON.parse(value);
        } catch {
            if (value === 'undefined') {
                return void 0;
            }
            return value;
        }
    }
}
