using System.Web.Http;
using WebActivatorEx;
using DictionaryApi;
using Swashbuckle.Application;
using System.Web.Http.Description;
using System.Globalization;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace DictionaryApi
{
    /// <summary>
    /// SwaggerConfig
    /// </summary>
    public class SwaggerConfig
    {
        /// <summary>
        /// Register
        /// </summary>
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    //c.SingleApiVersion("v1", "DictionaryApi");

                    // If your API has multiple versions, use "MultipleApiVersions" instead of "SingleApiVersion".
                    // In this case, you must provide a lambda that tells Swashbuckle which actions should be
                    // included in the docs for a given API version. Like "SingleApiVersion", each call to "Version"
                    // returns an "Info" builder so you can provide additional metadata per API version.
                    //
                    c.MultipleApiVersions(
                        ResolveVersionSupportByRouteConstraint,
                        (vc) =>
                        {
                            vc.Version("v1", "Dictionary API V1");
                            vc.Version("v2", "Dictionary API V2");

                        });
                    //c.MultipleApiVersions(
                    //(apiDesc, version) =>
                    //ResolveVersionSupportByRouteConstraint(apiDesc, version)


                    //,
                    //vc =>
                    //{
                    //    vc.Version("v1", "Swashbuckle Dummy API V1"); //add this line when v2 is released

                    //    // ReSharper disable once ConvertToLambdaExpression
                    //    vc.Version("v2", "Swashbuckle Dummy API V2");
                    //});
                    c.IncludeXmlComments($"{System.AppDomain.CurrentDomain.BaseDirectory}\\App_Data\\DictionaryApi.xml");
                })
                .EnableSwaggerUi(c =>
                {

                });
        }

        private static bool ResolveVersionSupportByRouteConstraint(ApiDescription apiDesc, string targetApiVersion)
        {
            var path = apiDesc.RelativePath.Split('/');
            var pathVersion = path[1];

            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(pathVersion, targetApiVersion, CompareOptions.IgnoreCase) >= 0;
        }
    }
}
