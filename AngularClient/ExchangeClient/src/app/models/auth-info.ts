import {UserDto} from './user.dto';

export interface AuthInfo {
    accessToken: string;
    refreshToken: string;
    userInfo: UserDto;
    serverUtcNow: string;
    accessUtcValidTo: string;
}
