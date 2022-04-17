import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

// const httpOPtions = {
//   headers : new HttpHeaders({
//     Authorization : 'Bearer '+JSON.parse(localStorage.getItem('user') || '{}')?.token
//   })
// }

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }
  //Angular has Services, to store state,Services are singleton, means same object will be served till application closes
  //there are other state mangement like Redux,but we are not using here
  members: Member[] = [];
  // getMembers() {
  //   return this.http.get<Member[]>(this.baseUrl + 'users',httpOPtions);
  // }
//since adding interceptor we dont need httpOPtions
  getMembers() {
    // return this.http.get<Member[]>(this.baseUrl + 'users');
    if (this.members.length > 0) return of(this.members);//of is in rxjs, which return observable,here we are returning local this.members array as observable
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members = members;
        return members;//map return observable 
      })
    )
  }

  getMember(username: string) {
    const member = this.members.find(x => x.username === username);//we are checking in local array
    if (member !== undefined) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }
  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    )
  }
}
