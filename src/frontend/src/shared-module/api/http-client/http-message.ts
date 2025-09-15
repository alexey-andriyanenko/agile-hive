import type { ExtractParams } from "./http-message.types";
import { HttpClient } from "./http-client";

export class HttpMessage<
  RequestBody,
  ResponseBody,
  Method extends "get" | "post" | "put" | "delete",
  Path extends string = string,
> {
  private _request: XMLHttpRequest = new XMLHttpRequest();

  private readonly _httpClient: HttpClient;

  private readonly _method: Method;

  private _url: string;

  private constructor(_url: string, _method: Method, _httpClient: HttpClient) {
    this._url = _url;
    this._method = _method;
    this._httpClient = _httpClient;
  }

  public static create<
    Request,
    Response,
    Method extends "get" | "post" | "put" | "delete",
    Path extends string = string,
  >(
    url: Path,
    method: Method,
    httpClient: HttpClient,
  ): HttpMessage<Request, Response, Method, Path> {
    return new HttpMessage<Request, Response, Method, Path>(url, method, httpClient);
  }

  public setHeaders(headers: Record<string, string>): Omit<this, "setHeaders"> {
    for (const key in headers) {
      this._request.setRequestHeader(key, headers[key]);
    }

    return this;
  }

  public setSearchParams(
    qp: Record<string, string | number | string[]>,
  ): Omit<this, "setSearchParams"> {
    const params = new URLSearchParams(this._stringifySearchParams(qp));
    this._url += `?${params.toString()}`;

    return this;
  }

  public setRouteParams(params: ExtractParams<Path>): Omit<this, "setRouteParams"> {
    const paramKeys = Object.keys(params);

    for (const key of paramKeys) {
      // @ts-expect-error TS cannot infer params type here
      this._url = this._url.replace(`:${key}`, params[key]);
    }

    return this;
  }

  public send<T extends Method>(
    body?: T extends "get" | "delete" ? never : RequestBody,
  ): Promise<ResponseBody> {
    return new Promise((resolve, reject) => {
      this._request.open(this._method, this._url);

      this._request.setRequestHeader("Content-Type", "application/json");

      if (HttpClient.accessToken)
        this._request.setRequestHeader("Authorization", `Bearer ${HttpClient.accessToken}`);

      this._request.onload = async () => {
        await Promise.all(
          this._httpClient.interceptors.map((interceptor) => {
            interceptor(this._request);
          }),
        );

        if (this._request.status >= 200 && this._request.status < 300) {
          const responseText = this._request.responseText ? this._request.responseText : "{}";
          resolve(JSON.parse(responseText));
        } else {
          try {
            const result = JSON.parse(this._request.responseText);
            reject(result);
            // eslint-disable-next-line @typescript-eslint/no-unused-vars
          } catch (e) {
            reject(this._request);
          }
        }
      };

      this._request.onerror = () => {
        try {
          const result = JSON.parse(this._request.responseText);
          reject(result);
          // eslint-disable-next-line @typescript-eslint/no-unused-vars
        } catch (e) {
          reject(this._request);
        }
      };

      this._request.send(JSON.stringify(body));
    });
  }

  private _stringifySearchParams(
    params: Record<string, string | number | string[]>,
  ): Record<string, string> {
    const result: Record<string, string> = {};

    for (const key in params) {
      const value = params[key];
      if (value === undefined || value === null) continue;
      if (Array.isArray(value)) {
        result[key] = value.join(",");
      } else {
        result[key] = String(value);
      }
    }

    return result;
  }
}
