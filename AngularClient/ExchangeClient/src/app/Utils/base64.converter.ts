export class Base64Converter {
  static convert(base64: string): any | null {
    return JSON.parse(decodeURIComponent(
      atob(base64)
        .split('')
        .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    ));
  }
}
