export namespace Anf{
    export class UrlHelper{
        public static isWebsite(str:string):boolean{
            return str.startsWith('http://')||
                str.startsWith('https://')||
                str.startsWith('www.');
        } 
        public static getUrl(str:string):string{
            if (str.startsWith('www.')) {
                return 'http://'+str;
            }
            return str;
        }
        public static fastGetHost(address:string):string{
            let len = address.length;
            let c='';
            let start = 0;
            let end = len;
            for (let i = 0; i < len; i++)
            {
                c = address[i];
                if ((c == '/') || (c == '?'))
                {
                    end = i;
                    break;
                }
                else if ((c == ':') && (len - i) > 3 && (address[i + 1] == '/') && (address[i + 2] == '/'))
                {
                    start = i + 3;
                    i = start;
                }
            }
            return address.substring(start, end);
        }
    }
}