type handler=(req:Request)=>Promise<Response> ;

interface RoutePart{
    route:string;
    handle:handler;
}


const getRouteMap:RoutePart[]=[
    {
        route:"get-searchs",
        handle:handleGetSerach
    }
];
export async function handleRequest(request: Request): Promise<Response> {
    const url=new URL(request.url);
    const pathName=url.pathname.split('/').filter(x=>x!='');
    const method=request.method.toLowerCase();
    if (method=='get') {
        const route=getHandler(pathName,getRouteMap);
        if (route) {
            return await route(request);
        }
    }
    return new Response(null,{
        status:404
    });
}
function getHandler(parts:string[],routes:RoutePart[]):handler|null {
    let eqRoutes=routes;
    for (let i = 0; i < parts.length; i++) {
        const ele = parts[i];
        eqRoutes=eqRoutes.filter((x,y)=>x.route==ele);
    }
    if (eqRoutes.length==0) {
        return null;
    }
    return eqRoutes[0].handle;
}
function handleGetSerach(request:Request) : Promise<Response>{
    return new Promise<Response>(()=> new Response(null));
}