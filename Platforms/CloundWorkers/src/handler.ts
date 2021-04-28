import { RedirectRequest} from './models'
export async function handleRequest(request: Request): Promise<Response> {
  const dataProm:Promise<RedirectRequest>=(request.method=='GET'||request.method=='get')?analysisGet(request):analysisPost(request);
  const data=await dataProm;
  const reqInit:RequestInit={
    method:data.method,
    headers:new Headers(data.headers)
  };
  if (data.method!='GET'&&data.method!='get') {
    reqInit.body=data.body;
  }
  return await fetch(data.url,reqInit);
}
async function analysisGet(request:Request):Promise<RedirectRequest> {
  const rurl=new URL(request.url);
  const url=rurl.searchParams.get('url');
  const method=rurl.searchParams.get('method');
  const body=rurl.searchParams.get('body');
  const req:RedirectRequest={
    url:url||'',
    method:method||'',
    body:body||'',
  };
  const usedInfo:Set<string>=new Set<string>(['url','method','body','headers']);
    var m=new Map<string,string>();
    for (const key in rurl.searchParams) {
      const ele = rurl.searchParams.get(key);
      if (!usedInfo.has(key)) {
        m.set(key,ele||'');
      }
    }
    if (m.size!=0) {
      req.headers=m;
    }
  return req;
}
async function analysisPost(request:Request):Promise<RedirectRequest> {
  const dts=await request.text();
  const data=<RedirectRequest>JSON.parse(dts);
  return data;
}