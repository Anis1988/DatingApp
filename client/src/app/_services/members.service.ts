import { environment } from './../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Member } from '../_models/member';
import { of } from 'rxjs';
import { map, mapTo } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members:Member[] = [];

  constructor(private http: HttpClient) {}

  getMembers() {
    if (this.members.length > 0 ) return of(this.members);
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members = members;
        return members;
      })
    );
  }
  getMember(username: string) {
    const member = this.members.find(x => x.username === username);
    if(member !== undefined) return of(member)
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }
  updateMember(member: Member) {
    return this.http.put<Member>(this.baseUrl + 'users/', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }
}
