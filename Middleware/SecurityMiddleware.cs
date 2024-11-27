namespace CoreContable.Middleware;

public class SecurityMiddleware(RequestDelegate next) {
    public async Task InvokeAsync(HttpContext context) {
        // Por alguna razon no funciona, cuando pasa por este punto el codigo siempre es 200.
        // Pero al llegar al front-end el status code es 401.
        // Asi que esta funcionalidad se maneja en el front-end.
        // Se deja el codigo por si en un futuro se necesita.
        if (context.Response.StatusCode == 401) {
            context.Response.Redirect("/security/login");
        }

        await next(context);
    }
}