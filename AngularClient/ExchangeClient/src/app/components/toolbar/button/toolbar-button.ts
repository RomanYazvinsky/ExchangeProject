import {Observable} from 'rxjs';

export interface ToolbarButton {
  isAvailable$: Observable<boolean>;
  isEnabled$: Observable<boolean>;
  label: string;
  executeAsync: () => Promise<any> | Observable<any> | void;
}
