using System;
using System.IO;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BucketService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace TasksService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; private set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            
            var awsOptions = Configuration.GetAWSOptions();
            awsOptions.Credentials = new EnvironmentVariablesAWSCredentials();
            awsOptions.Region = RegionEndpoint.CACentral1;
            services.AddDefaultAWSOptions(awsOptions);
            services.AddAWSService<IAmazonS3>();
           
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Tasks API", Version = "v1" });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, $"{PlatformServices.Default.Application.ApplicationName}.xml");
                c.IncludeXmlComments(xmlPath);
            });
            
            var builder = new ContainerBuilder();
            /*
            var s3Client = new AmazonS3Client(new EnvironmentVariablesAWSCredentials(), RegionEndpoint.CACentral1);
            builder.RegisterInstance(s3Client).As<IAmazonS3>();
            */
            builder.RegisterType<TasksS3Repository>().As<ITasksRepository>();
            builder.Populate(services);
            ApplicationContainer = builder.Build();

            

            return new AutofacServiceProvider(ApplicationContainer);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tasks API"); });
        }
    }
}
