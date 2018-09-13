using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;

namespace ConsoleApplication4
{

    class Program
    {
        static SecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

        //static string _plainTextSecurityKey = "This is my shared, not so secret, secret!";
        //static string _plainTextSecurityKey = "abc123";

        //static SecurityKey _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_plainTextSecurityKey));
        //static SecurityKey _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_plainTextSecurityKey));

        //static SigningCredentials _signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);
        //static SigningCredentials _signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.RsaSha256, SecurityAlgorithms.RsaSha256Signature);

        static void Main(string[] args)
        {
            //SecurityToken token = GetSampleJWT(out string signedAndEncodedToken);
            //SecurityToken token = GetLTILaunchJWTSharedKey(out string signedAndEncodedToken);

            //Console.WriteLine("-------------------------------------");
            //Console.WriteLine("LTI Launch JWT Token From LMS to LTI");
            //Console.WriteLine("-------------------------------------");
            //Console.WriteLine($"{JsonConvert.SerializeObject(token, Formatting.Indented)}");
            //Console.WriteLine();
            //Console.ReadLine();

            DemoData requestData = CreateLtiLaunchSymmetricKeyData();
            requestData.Dump();
            Console.ReadLine();

            DemoData validatedData = new DemoData
            {
                Token = requestData.Token, // normally would be validated here
                ValidationParameters = requestData.ValidationParameters,
            };
            validatedData.Dump();
            Console.ReadLine();

            //var tokenValidationParameters = new TokenValidationParameters()
            //{
            //    ValidAudiences = new string[]
            //    {
            //        "http://my.website.com",
            //        "http://my.otherwebsite.com",

            //        "10000000000001"
            //    },
            //    ValidIssuers = new string[]
            //    {
            //        "http://my.tokenissuer.com",
            //        "http://my.othertokenissuer.com",

            //        "http://canvas.instructure.com"
            //    },
            //    IssuerSigningKey = _signingKey
            //};

            ////_tokenHandler.ValidateToken(signedAndEncodedToken, tokenValidationParameters, out SecurityToken validatedToken);
            //SecurityToken validatedToken = token;

            //Console.WriteLine("-------------------------------");
            //Console.WriteLine("Validated LTI Launch JWT Token");
            //Console.WriteLine("-------------------------------");
            //Console.WriteLine();
            //Console.WriteLine(JsonConvert.SerializeObject(validatedToken, Formatting.Indented));
            //Console.ReadLine();
        }

        //static SecurityToken GetSampleJWT(out string signedAndEncodedToken)
        //{
        //    var claimsIdentity = new ClaimsIdentity(new List<Claim>()
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, "myemail@myprovider.com"),
        //        new Claim(ClaimTypes.Role, "Administrator"),
        //    }, "Custom");

        //    var securityTokenDescriptor = new SecurityTokenDescriptor()
        //    {
        //        Audience = "http://my.website.com",
        //        Issuer = "http://my.tokenissuer.com",
        //        Subject = claimsIdentity,
        //        SigningCredentials = _signingCredentials,
        //    };

        //    var securityToken = _tokenHandler.CreateToken(securityTokenDescriptor);

        //    signedAndEncodedToken = _tokenHandler.WriteToken(securityToken);

        //    Console.WriteLine(signedAndEncodedToken);
        //    Console.WriteLine();

        //    return securityToken;
        //}

        static SecurityToken GetLTILaunchJWTSharedKey(out string signedAndEncodedToken)
        {            
            // this JSON would be built up by Canvas and then encoded to a JWT

            string launchData = @"{
    ""https://purl.imsglobal.org/spec/lti/claim/message_type"":""LtiResourceLinkRequest"",
    ""https://purl.imsglobal.org/spec/lti/claim/version"":""1.3.0"",
    ""https://purl.imsglobal.org/spec/lti/claim/resource_link"":{
	    ""id"":""4dde05e8ca1973bcca9bffc13e1548820eee93a3""
    }, 
	""aud"":10000000000001,
	""https://purl.imsglobal.org/spec/lti/claim/deployment_id"":""1:8865aa05b4b79b64a91a86042e43af5ea8ae79eb"",
	""exp"":1536791390,
	""iat"":1536787790,
	""iss"":""https://canvas.instructure.com"",
	""nonce"":""e5e0a8ed-3004-45be-8382-2526a007dbe1"", 
	""sub"":""535fa085f22b4655f48cd5a36a9215f64c062838"", 
	""picture"":""http://canvas.instructure.com/images/messages/avatar-50.png"",
	""email"":""dmccallum@unicon.net"",
	""name"":""dmccallum@unicon.net"",
	""given_name"":""dmccallum@unicon.net"",
	""family_name"":"""",
	""https://purl.imsglobal.org/spec/lti/claim/lis"": {
		""person_sourcedid"":nil,
		""course_offering_sourcedid"":nil
    },
	""locale"":""en"",
	""https://purl.imsglobal.org/spec/lti/claim/roles"":[""http://purl.imsglobal.org/vocab/lis/v2/institution/person#Administrator""],
    ""https://purl.imsglobal.org/spec/lti/claim/context"":{
        ""id"":""4dde05e8ca1973bcca9bffc13e1548820eee93a3"",
		""label"":""DM Test 1"",
		""title"":""DM Test 1"",
		""type"":[""http://purl.imsglobal.org/vocab/lis/v2/course#CourseOffering""]
	},
	""https://purl.imsglobal.org/spec/lti/claim/tool_platform"":{
		""guid"":""OON8tfr5ABd4EKbaWbhlhiuMCJ0CeIQSUN6ikVA5:canvas-lms"",
		""name"":""LTI Advantage"",
		""version"":""cloud"",
		""product_family_code"":""canvas""
	},
	""https://purl.imsglobal.org/spec/lti/claim/launch_presentation"":{
		""document_target"":""iframe"",
		""height"":400,
		""width"":800,
		""return_url"":""http://canvas.docker/courses/1/external_content/success/external_tool_redirect"",
		""locale"":""en""
	}, 
	""https://purl.imsglobal.org/spec/lti/claim/custom"":{},
	""https://www.instructure.com/canvas_user_id"":1,
	""https://www.instructure.com/canvas_user_login_id"":""dmccallum@unicon.net"",
	""https://www.instructure.com/canvas_api_domain"":""canvas.docker"",
	""https://www.instructure.com/canvas_course_id"":1,
	""https://www.instructure.com/canvas_workflow_state"":""claimed"",
	""https://www.instructure.com/lis_course_offering_sourcedid"":nil,
	""https://www.instructure.com/roles"":""urn:lti:instrole:ims/lis/Administrator,urn:lti:sysrole:ims/lis/SysAdmin,urn:lti:sysrole:ims/lis/User"",
    ""https://www.instructure.com/canvas_enrollment_state"":""
}";
            signedAndEncodedToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IjIwMTgtMDYtMThUMjI6MzM6MjBaIn0.eyJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9tZXNzYWdlX3R5cGUiOiJMdGlSZXNvdXJjZUxpbmtSZXF1ZXN0IiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vdmVyc2lvbiI6IjEuMy4wIiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vcmVzb3VyY2VfbGluayI6eyJpZCI6IjRkZGUwNWU4Y2ExOTczYmNjYTliZmZjMTNlMTU0ODgyMGVlZTkzYTMifSwiYXVkIjoxMDAwMDAwMDAwMDAwMSwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vZGVwbG95bWVudF9pZCI6IjE6ODg2NWFhMDViNGI3OWI2NGE5MWE4NjA0MmU0M2FmNWVhOGFlNzllYiIsImV4cCI6MTUzNjc5MTM5MCwiaWF0IjoxNTM2Nzg3NzkwLCJpc3MiOiJodHRwczovL2NhbnZhcy5pbnN0cnVjdHVyZS5jb20iLCJub25jZSI6ImU1ZTBhOGVkLTMwMDQtNDViZS04MzgyLTI1MjZhMDA3ZGJlMSIsInN1YiI6IjUzNWZhMDg1ZjIyYjQ2NTVmNDhjZDVhMzZhOTIxNWY2NGMwNjI4MzgiLCJwaWN0dXJlIjoiaHR0cDovL2NhbnZhcy5pbnN0cnVjdHVyZS5jb20vaW1hZ2VzL21lc3NhZ2VzL2F2YXRhci01MC5wbmciLCJlbWFpbCI6ImRtY2NhbGx1bUB1bmljb24ubmV0IiwibmFtZSI6ImRtY2NhbGx1bUB1bmljb24ubmV0IiwiZ2l2ZW5fbmFtZSI6ImRtY2NhbGx1bUB1bmljb24ubmV0IiwiZmFtaWx5X25hbWUiOiIiLCJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9saXMiOnsicGVyc29uX3NvdXJjZWRpZCI6bnVsbCwiY291cnNlX29mZmVyaW5nX3NvdXJjZWRpZCI6bnVsbH0sImxvY2FsZSI6ImVuIiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vcm9sZXMiOlsiaHR0cDovL3B1cmwuaW1zZ2xvYmFsLm9yZy92b2NhYi9saXMvdjIvaW5zdGl0dXRpb24vcGVyc29uI0FkbWluaXN0cmF0b3IiXSwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vY29udGV4dCI6eyJpZCI6IjRkZGUwNWU4Y2ExOTczYmNjYTliZmZjMTNlMTU0ODgyMGVlZTkzYTMiLCJsYWJlbCI6IkRNIFRlc3QgMSIsInRpdGxlIjoiRE0gVGVzdCAxIiwidHlwZSI6WyJodHRwOi8vcHVybC5pbXNnbG9iYWwub3JnL3ZvY2FiL2xpcy92Mi9jb3Vyc2UjQ291cnNlT2ZmZXJpbmciXX0sImh0dHBzOi8vcHVybC5pbXNnbG9iYWwub3JnL3NwZWMvbHRpL2NsYWltL3Rvb2xfcGxhdGZvcm0iOnsiZ3VpZCI6Ik9PTjh0ZnI1QUJkNEVLYmFXYmhsaGl1TUNKMENlSVFTVU42aWtWQTU6Y2FudmFzLWxtcyIsIm5hbWUiOiJMVEkgQWR2YW50YWdlIiwidmVyc2lvbiI6ImNsb3VkIiwicHJvZHVjdF9mYW1pbHlfY29kZSI6ImNhbnZhcyJ9LCJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9sYXVuY2hfcHJlc2VudGF0aW9uIjp7ImRvY3VtZW50X3RhcmdldCI6ImlmcmFtZSIsImhlaWdodCI6NDAwLCJ3aWR0aCI6ODAwLCJyZXR1cm5fdXJsIjoiaHR0cDovL2NhbnZhcy5kb2NrZXIvY291cnNlcy8xL2V4dGVybmFsX2NvbnRlbnQvc3VjY2Vzcy9leHRlcm5hbF90b29sX3JlZGlyZWN0IiwibG9jYWxlIjoiZW4ifSwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vY3VzdG9tIjp7fSwiaHR0cHM6Ly93d3cuaW5zdHJ1Y3R1cmUuY29tL2NhbnZhc191c2VyX2lkIjoxLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vY2FudmFzX3VzZXJfbG9naW5faWQiOiJkbWNjYWxsdW1AdW5pY29uLm5ldCIsImh0dHBzOi8vd3d3Lmluc3RydWN0dXJlLmNvbS9jYW52YXNfYXBpX2RvbWFpbiI6ImNhbnZhcy5kb2NrZXIiLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vY2FudmFzX2NvdXJzZV9pZCI6MSwiaHR0cHM6Ly93d3cuaW5zdHJ1Y3R1cmUuY29tL2NhbnZhc193b3JrZmxvd19zdGF0ZSI6ImNsYWltZWQiLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vbGlzX2NvdXJzZV9vZmZlcmluZ19zb3VyY2VkaWQiOm51bGwsImh0dHBzOi8vd3d3Lmluc3RydWN0dXJlLmNvbS9yb2xlcyI6InVybjpsdGk6aW5zdHJvbGU6aW1zL2xpcy9BZG1pbmlzdHJhdG9yLHVybjpsdGk6c3lzcm9sZTppbXMvbGlzL1N5c0FkbWluLHVybjpsdGk6c3lzcm9sZTppbXMvbGlzL1VzZXIiLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vY2FudmFzX2Vucm9sbG1lbnRfc3RhdGUiOiIifQ.c9Qay0rCJy0zE9Td2_jSkN0NqueC0hdFhr50lyc_NombVFzPV4dFEVpiCn_8BGB9yDcAQedXj-KBf6uvcgdmjA";
            return _tokenHandler.ReadToken(signedAndEncodedToken);
        }

        static DemoData CreateSampleSymmetricKeyData()
        {
            DemoData data = new DemoData();

            data.Name = "Sample symmetric key";
            data.SigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("This is my shared, not so secret, secret!"));
            data.SigningCredentials = new SigningCredentials(data.SigningKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);
            data.ValidationParameters = new TokenValidationParameters()
            {
                ValidAudiences = new string[]
                {
                    "http://my.website.com",
                    "http://my.otherwebsite.com",
                },
                ValidIssuers = new string[]
                {
                    "http://my.tokenissuer.com",
                    "http://my.othertokenissuer.com",
                },
                IssuerSigningKey = data.SigningKey
            };

            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, "myemail@myprovider.com"),
                    new Claim(ClaimTypes.Role, "Administrator"),
                }, "Custom");

            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Audience = "http://my.website.com",
                Issuer = "http://my.tokenissuer.com",
                Subject = claimsIdentity,
                SigningCredentials = data.SigningCredentials
            };

            data.Token = _tokenHandler.CreateToken(securityTokenDescriptor);
            data.SignedEncodedToken = _tokenHandler.WriteToken(data.Token);

            return data;
        }

        static DemoData CreateLtiLaunchSymmetricKeyData()
        {
            /*
                Step 0: Set up an IMS Reference Tool

                Name: PSU LTI1.3DotNet
                Client: 094538B8-3E5F-4713-8EAC-B14B8FCDF9BE
                Private key: abc123 (shared secret)
                Deployment: https://localhost/Default.aspx
                Keyset url:
                oauth2 url:
            */

            DemoData data = new DemoData { Name = "LTI Launch JWT Token From LMS to LTI" };

            /* 
                Step 1: Found a Tool Launch for the tool in IMS Ref Tool (from User: dmccallum@unicon.net).
            */

            // raw JWT containing all LTI launch data. This would come from Canvas
            data.SignedEncodedToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IjIwMTgtMDYtMThUMjI6MzM6MjBaIn0.eyJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9tZXNzYWdlX3R5cGUiOiJMdGlSZXNvdXJjZUxpbmtSZXF1ZXN0IiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vdmVyc2lvbiI6IjEuMy4wIiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vcmVzb3VyY2VfbGluayI6eyJpZCI6IjRkZGUwNWU4Y2ExOTczYmNjYTliZmZjMTNlMTU0ODgyMGVlZTkzYTMifSwiYXVkIjoxMDAwMDAwMDAwMDAwMSwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vZGVwbG95bWVudF9pZCI6IjE6ODg2NWFhMDViNGI3OWI2NGE5MWE4NjA0MmU0M2FmNWVhOGFlNzllYiIsImV4cCI6MTUzNjc5MTM5MCwiaWF0IjoxNTM2Nzg3NzkwLCJpc3MiOiJodHRwczovL2NhbnZhcy5pbnN0cnVjdHVyZS5jb20iLCJub25jZSI6ImU1ZTBhOGVkLTMwMDQtNDViZS04MzgyLTI1MjZhMDA3ZGJlMSIsInN1YiI6IjUzNWZhMDg1ZjIyYjQ2NTVmNDhjZDVhMzZhOTIxNWY2NGMwNjI4MzgiLCJwaWN0dXJlIjoiaHR0cDovL2NhbnZhcy5pbnN0cnVjdHVyZS5jb20vaW1hZ2VzL21lc3NhZ2VzL2F2YXRhci01MC5wbmciLCJlbWFpbCI6ImRtY2NhbGx1bUB1bmljb24ubmV0IiwibmFtZSI6ImRtY2NhbGx1bUB1bmljb24ubmV0IiwiZ2l2ZW5fbmFtZSI6ImRtY2NhbGx1bUB1bmljb24ubmV0IiwiZmFtaWx5X25hbWUiOiIiLCJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9saXMiOnsicGVyc29uX3NvdXJjZWRpZCI6bnVsbCwiY291cnNlX29mZmVyaW5nX3NvdXJjZWRpZCI6bnVsbH0sImxvY2FsZSI6ImVuIiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vcm9sZXMiOlsiaHR0cDovL3B1cmwuaW1zZ2xvYmFsLm9yZy92b2NhYi9saXMvdjIvaW5zdGl0dXRpb24vcGVyc29uI0FkbWluaXN0cmF0b3IiXSwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vY29udGV4dCI6eyJpZCI6IjRkZGUwNWU4Y2ExOTczYmNjYTliZmZjMTNlMTU0ODgyMGVlZTkzYTMiLCJsYWJlbCI6IkRNIFRlc3QgMSIsInRpdGxlIjoiRE0gVGVzdCAxIiwidHlwZSI6WyJodHRwOi8vcHVybC5pbXNnbG9iYWwub3JnL3ZvY2FiL2xpcy92Mi9jb3Vyc2UjQ291cnNlT2ZmZXJpbmciXX0sImh0dHBzOi8vcHVybC5pbXNnbG9iYWwub3JnL3NwZWMvbHRpL2NsYWltL3Rvb2xfcGxhdGZvcm0iOnsiZ3VpZCI6Ik9PTjh0ZnI1QUJkNEVLYmFXYmhsaGl1TUNKMENlSVFTVU42aWtWQTU6Y2FudmFzLWxtcyIsIm5hbWUiOiJMVEkgQWR2YW50YWdlIiwidmVyc2lvbiI6ImNsb3VkIiwicHJvZHVjdF9mYW1pbHlfY29kZSI6ImNhbnZhcyJ9LCJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9sYXVuY2hfcHJlc2VudGF0aW9uIjp7ImRvY3VtZW50X3RhcmdldCI6ImlmcmFtZSIsImhlaWdodCI6NDAwLCJ3aWR0aCI6ODAwLCJyZXR1cm5fdXJsIjoiaHR0cDovL2NhbnZhcy5kb2NrZXIvY291cnNlcy8xL2V4dGVybmFsX2NvbnRlbnQvc3VjY2Vzcy9leHRlcm5hbF90b29sX3JlZGlyZWN0IiwibG9jYWxlIjoiZW4ifSwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vY3VzdG9tIjp7fSwiaHR0cHM6Ly93d3cuaW5zdHJ1Y3R1cmUuY29tL2NhbnZhc191c2VyX2lkIjoxLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vY2FudmFzX3VzZXJfbG9naW5faWQiOiJkbWNjYWxsdW1AdW5pY29uLm5ldCIsImh0dHBzOi8vd3d3Lmluc3RydWN0dXJlLmNvbS9jYW52YXNfYXBpX2RvbWFpbiI6ImNhbnZhcy5kb2NrZXIiLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vY2FudmFzX2NvdXJzZV9pZCI6MSwiaHR0cHM6Ly93d3cuaW5zdHJ1Y3R1cmUuY29tL2NhbnZhc193b3JrZmxvd19zdGF0ZSI6ImNsYWltZWQiLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vbGlzX2NvdXJzZV9vZmZlcmluZ19zb3VyY2VkaWQiOm51bGwsImh0dHBzOi8vd3d3Lmluc3RydWN0dXJlLmNvbS9yb2xlcyI6InVybjpsdGk6aW5zdHJvbGU6aW1zL2xpcy9BZG1pbmlzdHJhdG9yLHVybjpsdGk6c3lzcm9sZTppbXMvbGlzL1N5c0FkbWluLHVybjpsdGk6c3lzcm9sZTppbXMvbGlzL1VzZXIiLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vY2FudmFzX2Vucm9sbG1lbnRfc3RhdGUiOiIifQ.c9Qay0rCJy0zE9Td2_jSkN0NqueC0hdFhr50lyc_NombVFzPV4dFEVpiCn_8BGB9yDcAQedXj-KBf6uvcgdmjA"; ;

            /*
                Step 2: Tool validates the token
            */
            
            // signing key generated using, the shared secret from the tool set up in Step 0
            data.SigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abc123"));
            
            // validation parameters, using values under Security Details
            data.ValidationParameters = new TokenValidationParameters()
            {
                ValidAudiences = new string[]
                {
                    "10000000000001" // "aud": 10000000000001
                },
                ValidIssuers = new string[]
                {
                    "http://canvas.instructure.com" // "iss": "https://canvas.instructure.com"
                },
                IssuerSigningKey = data.SigningKey
            };

            // signing credentials, using the algorithm corresponding to "alg": ""RS256" in the launch
            data.SigningCredentials = new SigningCredentials(data.SigningKey, SecurityAlgorithms.RsaSha256, SecurityAlgorithms.RsaSha256Signature);

            data.Token = _tokenHandler.ReadToken(data.SignedEncodedToken);
            //_tokenHandler.ValidateToken(signedAndEncodedToken, tokenValidationParameters, out SecurityToken validatedToken);
            //data.Token = validated token

            return data;
        }
    }

    public class DemoData
    {
        public string Name { get; set; }
        public SecurityKey SigningKey { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
        public SecurityToken Token { get; set; }
        public string SignedEncodedToken { get; set; }
        public TokenValidationParameters ValidationParameters { get; set; }
        public void Dump()
        {
            Console.WriteLine("-------------------------------------");
            Console.WriteLine(Name);
            Console.WriteLine("-------------------------------------");
            Console.WriteLine($"{JsonConvert.SerializeObject(Token, Formatting.Indented)}");
            Console.WriteLine();
        }
    }

}
