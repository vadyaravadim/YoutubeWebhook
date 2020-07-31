using YoutubeWebhook.Notifications;
using YoutubeWebhook.Interfaces;
using YoutubeWebhook.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace YoutubeWebhook
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddHttpClient<IWebHookYoutube, ImplementationSubscribe>();
            services.AddHttpClient<INotificationTelegram, TelegramNotification>();
            services.AddSingleton((instance) =>
            {
                return new YoutubeRequestData()
                {
                    CallbackUrl = "http://yourUrl/youtubehook/", // set url current app deployment for notify youtube
                    SubscribeUrl = "https://pubsubhubbub.appspot.com/subscribe",
                    TopicUrl = "https://www.youtube.com/xml/feeds/videos.xml?channel_id="
                };
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "YoutubeSubscriptionPlatform");
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
