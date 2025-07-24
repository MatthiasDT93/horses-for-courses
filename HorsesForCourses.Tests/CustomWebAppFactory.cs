using System.IO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using HorsesForCourses.WebApi;

namespace HorsesForCourses.Tests;

internal class CustomWebAppFactory : WebApplicationFactory<Program> { }