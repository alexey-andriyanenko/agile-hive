import { HttpMessage } from "./http-message";

type HttpClientInterceptor = (request: XMLHttpRequest) => Promise<void> | void;

type HttpClientConfig = {
  baseUrl: string;
  anonymous?: boolean;
  interceptors?: HttpClientInterceptor[];
};

export class HttpClient {
  private _config: HttpClientConfig;

  constructor(_config: HttpClientConfig) {
    this._config = _config;
  }

  public static accessToken = localStorage.getItem("accessToken");

  public static refreshToken = localStorage.getItem("refreshToken");

  public get interceptors(): HttpClientInterceptor[] {
    return this._config.interceptors ?? [];
  }

  public get<Response, Path extends string = string>(url: Path) {
    return HttpMessage.create<never, Response, "get", Path>(
      (this._config.baseUrl + url) as Path,
      "get",
      this,
    );
  }

  public post<Request, Response, Path extends string = string>(url: Path) {
    return HttpMessage.create<Request, Response, "post", Path>(
      (this._config.baseUrl + url) as Path,
      "post",
      this,
    );
  }

  public put<Request, Response, Path extends string = string>(url: Path) {
    return HttpMessage.create<Request, Response, "put", Path>(
      (this._config.baseUrl + url) as Path,
      "put",
      this,
    );
  }

  public delete<Response, Path extends string = string>(url: Path) {
    return HttpMessage.create<never, Response, "delete", Path>(
      (this._config.baseUrl + url) as Path,
      "delete",
      this,
    );
  }
}
