using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Repo.Supports
{
    public class FileResponseOperationFilter : IOperationFilter
    {   
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var isFileDownload = context.MethodInfo.Name.ToLower().Contains("download");

            if (!isFileDownload)
                return;

            operation.Responses["200"] = new OpenApiResponse
            {
                Description = "File downloaded successfully",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/octet-stream"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        }
                    }
                }
            };
        }
    }
}
