namespace ASP_NET_Final_Proj.Extensions;

public static class PipelineExtensions
{
    public static WebApplication UseTaskFlowPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                options.RoutePrefix = string.Empty;
                options.EnableFilter();
                options.EnableTryItOutByDefault();
                options.DisplayRequestDuration();
                options.EnablePersistAuthorization();
            });
            app.MapOpenApi();
        }
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
        
    }
}
