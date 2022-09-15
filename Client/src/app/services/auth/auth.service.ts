import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ILoginResponse } from 'src/app/login/login-response.interface';

const AUTH_API = 'https://localhost:7125/JWTAuth/';
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
};

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<ILoginResponse> {
    return this.http.post<ILoginResponse>(
      AUTH_API + 'login',
      {
        EmailAddress: email,
        Password: password,
      },
      httpOptions
    );
  }
}
