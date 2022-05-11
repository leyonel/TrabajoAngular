/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';
import { RequestBuilder } from '../request-builder';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';

import { WeatherForecast } from '../models/weather-forecast';

@Injectable({
  providedIn: 'root',
})
export class WeatherForecastService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation getWeatherForecast
   */
  static readonly GetWeatherForecastPath = '/WeatherForecast';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getWeatherForecast$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  getWeatherForecast$Plain$Response(params?: {
  }): Observable<StrictHttpResponse<Array<WeatherForecast>>> {

    const rb = new RequestBuilder(this.rootUrl, WeatherForecastService.GetWeatherForecastPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<WeatherForecast>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getWeatherForecast$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getWeatherForecast$Plain(params?: {
  }): Observable<Array<WeatherForecast>> {

    return this.getWeatherForecast$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<Array<WeatherForecast>>) => r.body as Array<WeatherForecast>)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getWeatherForecast$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  getWeatherForecast$Json$Response(params?: {
  }): Observable<StrictHttpResponse<Array<WeatherForecast>>> {

    const rb = new RequestBuilder(this.rootUrl, WeatherForecastService.GetWeatherForecastPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<WeatherForecast>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getWeatherForecast$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getWeatherForecast$Json(params?: {
  }): Observable<Array<WeatherForecast>> {

    return this.getWeatherForecast$Json$Response(params).pipe(
      map((r: StrictHttpResponse<Array<WeatherForecast>>) => r.body as Array<WeatherForecast>)
    );
  }

}
