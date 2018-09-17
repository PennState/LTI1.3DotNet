using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace ConsoleApplication4
{
    class Program
    {
        static SecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

        static void Main(string[] args)
        {
            DemoData requestData = CreateLtiLaunchSymmetricKeyData();
            //DemoData requestData = CreateLtiLaunchAsymmetricKeyData();

            //requestData.Dump();
            //Console.ReadLine();            

            // here we would grab useful stuff out of the token

            requestData.Validate(_tokenHandler);

            requestData.Dump();

            Console.ReadLine();
        }

        static SecurityToken GetLTILaunchJWTSharedKey(out string signedAndEncodedToken)
        {
            // this JSON would be built up by Canvas and then encoded to a JWT

            #region
            /*
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
*/
            #endregion

            //signedAndEncodedToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IjIwMTgtMDYtMThUMjI6MzM6MjBaIn0.eyJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9tZXNzYWdlX3R5cGUiOiJMdGlSZXNvdXJjZUxpbmtSZXF1ZXN0IiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vdmVyc2lvbiI6IjEuMy4wIiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vcmVzb3VyY2VfbGluayI6eyJpZCI6IjRkZGUwNWU4Y2ExOTczYmNjYTliZmZjMTNlMTU0ODgyMGVlZTkzYTMifSwiYXVkIjoxMDAwMDAwMDAwMDAwMSwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vZGVwbG95bWVudF9pZCI6IjE6ODg2NWFhMDViNGI3OWI2NGE5MWE4NjA0MmU0M2FmNWVhOGFlNzllYiIsImV4cCI6MTUzNjc5MTM5MCwiaWF0IjoxNTM2Nzg3NzkwLCJpc3MiOiJodHRwczovL2NhbnZhcy5pbnN0cnVjdHVyZS5jb20iLCJub25jZSI6ImU1ZTBhOGVkLTMwMDQtNDViZS04MzgyLTI1MjZhMDA3ZGJlMSIsInN1YiI6IjUzNWZhMDg1ZjIyYjQ2NTVmNDhjZDVhMzZhOTIxNWY2NGMwNjI4MzgiLCJwaWN0dXJlIjoiaHR0cDovL2NhbnZhcy5pbnN0cnVjdHVyZS5jb20vaW1hZ2VzL21lc3NhZ2VzL2F2YXRhci01MC5wbmciLCJlbWFpbCI6ImRtY2NhbGx1bUB1bmljb24ubmV0IiwibmFtZSI6ImRtY2NhbGx1bUB1bmljb24ubmV0IiwiZ2l2ZW5fbmFtZSI6ImRtY2NhbGx1bUB1bmljb24ubmV0IiwiZmFtaWx5X25hbWUiOiIiLCJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9saXMiOnsicGVyc29uX3NvdXJjZWRpZCI6bnVsbCwiY291cnNlX29mZmVyaW5nX3NvdXJjZWRpZCI6bnVsbH0sImxvY2FsZSI6ImVuIiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vcm9sZXMiOlsiaHR0cDovL3B1cmwuaW1zZ2xvYmFsLm9yZy92b2NhYi9saXMvdjIvaW5zdGl0dXRpb24vcGVyc29uI0FkbWluaXN0cmF0b3IiXSwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vY29udGV4dCI6eyJpZCI6IjRkZGUwNWU4Y2ExOTczYmNjYTliZmZjMTNlMTU0ODgyMGVlZTkzYTMiLCJsYWJlbCI6IkRNIFRlc3QgMSIsInRpdGxlIjoiRE0gVGVzdCAxIiwidHlwZSI6WyJodHRwOi8vcHVybC5pbXNnbG9iYWwub3JnL3ZvY2FiL2xpcy92Mi9jb3Vyc2UjQ291cnNlT2ZmZXJpbmciXX0sImh0dHBzOi8vcHVybC5pbXNnbG9iYWwub3JnL3NwZWMvbHRpL2NsYWltL3Rvb2xfcGxhdGZvcm0iOnsiZ3VpZCI6Ik9PTjh0ZnI1QUJkNEVLYmFXYmhsaGl1TUNKMENlSVFTVU42aWtWQTU6Y2FudmFzLWxtcyIsIm5hbWUiOiJMVEkgQWR2YW50YWdlIiwidmVyc2lvbiI6ImNsb3VkIiwicHJvZHVjdF9mYW1pbHlfY29kZSI6ImNhbnZhcyJ9LCJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9sYXVuY2hfcHJlc2VudGF0aW9uIjp7ImRvY3VtZW50X3RhcmdldCI6ImlmcmFtZSIsImhlaWdodCI6NDAwLCJ3aWR0aCI6ODAwLCJyZXR1cm5fdXJsIjoiaHR0cDovL2NhbnZhcy5kb2NrZXIvY291cnNlcy8xL2V4dGVybmFsX2NvbnRlbnQvc3VjY2Vzcy9leHRlcm5hbF90b29sX3JlZGlyZWN0IiwibG9jYWxlIjoiZW4ifSwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vY3VzdG9tIjp7fSwiaHR0cHM6Ly93d3cuaW5zdHJ1Y3R1cmUuY29tL2NhbnZhc191c2VyX2lkIjoxLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vY2FudmFzX3VzZXJfbG9naW5faWQiOiJkbWNjYWxsdW1AdW5pY29uLm5ldCIsImh0dHBzOi8vd3d3Lmluc3RydWN0dXJlLmNvbS9jYW52YXNfYXBpX2RvbWFpbiI6ImNhbnZhcy5kb2NrZXIiLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vY2FudmFzX2NvdXJzZV9pZCI6MSwiaHR0cHM6Ly93d3cuaW5zdHJ1Y3R1cmUuY29tL2NhbnZhc193b3JrZmxvd19zdGF0ZSI6ImNsYWltZWQiLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vbGlzX2NvdXJzZV9vZmZlcmluZ19zb3VyY2VkaWQiOm51bGwsImh0dHBzOi8vd3d3Lmluc3RydWN0dXJlLmNvbS9yb2xlcyI6InVybjpsdGk6aW5zdHJvbGU6aW1zL2xpcy9BZG1pbmlzdHJhdG9yLHVybjpsdGk6c3lzcm9sZTppbXMvbGlzL1N5c0FkbWluLHVybjpsdGk6c3lzcm9sZTppbXMvbGlzL1VzZXIiLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vY2FudmFzX2Vucm9sbG1lbnRfc3RhdGUiOiIifQ.c9Qay0rCJy0zE9Td2_jSkN0NqueC0hdFhr50lyc_NombVFzPV4dFEVpiCn_8BGB9yDcAQedXj-KBf6uvcgdmjA";
            signedAndEncodedToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IlZhNWFpcUtlWHZBMTlaRTlTYjl3clJVcVEwbHpnRFVCN2wyWjM1RVR3Yk0ifQ.eyJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9tZXNzYWdlX3R5cGUiOiJMdGlEZWVwTGlua2luZ1JlcXVlc3QiLCJnaXZlbl9uYW1lIjoiTGFuaSIsImZhbWlseV9uYW1lIjoiV2F0c2ljYSIsIm1pZGRsZV9uYW1lIjoiQXNoYSIsInBpY3R1cmUiOiJodHRwOi8vZXhhbXBsZS5vcmcvTGFuaS5qcGciLCJlbWFpbCI6IkxhbmkuV2F0c2ljYUBleGFtcGxlLm9yZyIsIm5hbWUiOiJMYW5pIEFzaGEgV2F0c2ljYSIsImh0dHBzOi8vcHVybC5pbXNnbG9iYWwub3JnL3NwZWMvbHRpL2NsYWltL3JvbGVzIjpbImh0dHA6Ly9wdXJsLmltc2dsb2JhbC5vcmcvdm9jYWIvbGlzL3YyL2luc3RpdHV0aW9uL3BlcnNvbiNJbnN0cnVjdG9yIl0sImh0dHBzOi8vcHVybC5pbXNnbG9iYWwub3JnL3NwZWMvbHRpL2NsYWltL3JvbGVfc2NvcGVfbWVudG9yIjpbImh0dHA6Ly9wdXJsLmltc2dsb2JhbC5vcmcvdm9jYWIvbGlzL3YyL2luc3RpdHV0aW9uL3BlcnNvbiNBZG1pbmlzdHJhdG9yIl0sImh0dHBzOi8vcHVybC5pbXNnbG9iYWwub3JnL3NwZWMvbHRpL2NsYWltL2NvbnRleHQiOnsiaWQiOiIyNSIsImxhYmVsIjoiZW5nXzEwMSIsInRpdGxlIjoiRW5nIDEwMSIsInR5cGUiOlsiQ291cnNlT2ZmZXJpbmciXX0sImh0dHBzOi8vcHVybC5pbXNnbG9iYWwub3JnL3NwZWMvbHRpL2NsYWltL3Rvb2xfcGxhdGZvcm0iOnsibmFtZSI6IkphbWVzIFRlc3QgUGxhdGZvcm0iLCJjb250YWN0X2VtYWlsIjoiIiwiZGVzY3JpcHRpb24iOiIiLCJ1cmwiOiIiLCJwcm9kdWN0X2ZhbWlseV9jb2RlIjoiIiwidmVyc2lvbiI6IjEuMCJ9LCJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS1kbC9jbGFpbS9kZWVwX2xpbmtpbmdfc2V0dGluZ3MiOnsiYWNjZXB0X3R5cGVzIjpbImxpbmsiLCJmaWxlIiwiaHRtbCIsImx0aUxpbmsiLCJpbWFnZSJdLCJhY2NlcHRfbWVkaWFfdHlwZXMiOiJpbWFnZS8qLHRleHQvaHRtbCIsImFjY2VwdF9wcmVzZW50YXRpb25fZG9jdW1lbnRfdGFyZ2V0cyI6WyJpZnJhbWUiLCJ3aW5kb3ciLCJlbWJlZCJdLCJhY2NlcHRfbXVsdGlwbGUiOnRydWUsImF1dG9fY3JlYXRlIjp0cnVlLCJ0aXRsZSI6IlRoaXMgaXMgdGhlIGRlZmF1bHQgdGl0bGUiLCJ0ZXh0IjoiVGhpcyBpcyB0aGUgZGVmYXVsdCB0ZXh0IiwiZGF0YSI6IlNvbWUgcmFuZG9tIG9wYXF1ZSBkYXRhIHRoYXQgTVVTVCBiZSBzZW50IGJhY2siLCJkZWVwX2xpbmtfcmV0dXJuX3VybCI6Imh0dHBzOi8vbHRpLXJpLmltc2dsb2JhbC5vcmcvcGxhdGZvcm1zLzI3L2NvbnRleHRzLzI1L2RlZXBfbGlua3MifSwiaXNzIjoiaHR0cHM6Ly9sdGktcmkuaW1zZ2xvYmFsLm9yZyIsImF1ZCI6IjEyMzQ1IiwiaWF0IjoxNTM3MTk1ODYyLCJleHAiOjE1MzcxOTYxNjIsInN1YiI6ImViMGZmNDQ1MDhiOTNjMzExN2FlIiwibm9uY2UiOiI4OGFmOTgwODg2Y2E4MzE4Y2MxOCIsImh0dHBzOi8vcHVybC5pbXNnbG9iYWwub3JnL3NwZWMvbHRpL2NsYWltL3ZlcnNpb24iOiIxLjMuMCIsImxvY2FsZSI6ImVuLVVTIiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vbGF1bmNoX3ByZXNlbnRhdGlvbiI6eyJkb2N1bWVudF90YXJnZXQiOiJpZnJhbWUiLCJoZWlnaHQiOjMyMCwid2lkdGgiOjI0MH0sImh0dHBzOi8vd3d3LmV4YW1wbGUuY29tL2V4dGVuc2lvbiI6eyJjb2xvciI6InZpb2xldCJ9LCJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9jdXN0b20iOnsibXlDdXN0b21WYWx1ZSI6IjEyMyJ9LCJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9kZXBsb3ltZW50X2lkIjoiMTIzIn0.J4QZbIbxEz2QVXgAIqyqS-hQsPZqX3WWeNVGtSpneYdPHSxe28s2kUI4_fkBzJ6KWu1gDodkXMHc5Yh_P5236IjNbz_ZiFcftzlOgKG-jBmD5x5YWsKksXYKLLRik5iLiFX_UKmCOxNINpqC6BMES4k2QqbstqTYLbyNzco_1vEJlaCyROCHU9U2fD6MY90yaxfGDil_EujOrXZva6H8mNhS5DrzLvBHaJIBOELjkytVGjGMA9_Bkn6b368HGq1viI-S5MJY4JQTIOj9IJYuKGWy_XPit7yWKl-JciZnaeF3OwQsDU7htdZXuwwl_mRu0bNA_kKS-mIQuP6ZFEQcdw";

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
//            data.SignedEncodedToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IjIwMTgtMDYtMThUMjI6MzM6MjBaIn0.eyJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9tZXNzYWdlX3R5cGUiOiJMdGlSZXNvdXJjZUxpbmtSZXF1ZXN0IiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vdmVyc2lvbiI6IjEuMy4wIiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vcmVzb3VyY2VfbGluayI6eyJpZCI6IjRkZGUwNWU4Y2ExOTczYmNjYTliZmZjMTNlMTU0ODgyMGVlZTkzYTMifSwiYXVkIjoxMDAwMDAwMDAwMDAwMSwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vZGVwbG95bWVudF9pZCI6IjE6ODg2NWFhMDViNGI3OWI2NGE5MWE4NjA0MmU0M2FmNWVhOGFlNzllYiIsImV4cCI6MTUzNjc5MTM5MCwiaWF0IjoxNTM2Nzg3NzkwLCJpc3MiOiJodHRwczovL2NhbnZhcy5pbnN0cnVjdHVyZS5jb20iLCJub25jZSI6ImU1ZTBhOGVkLTMwMDQtNDViZS04MzgyLTI1MjZhMDA3ZGJlMSIsInN1YiI6IjUzNWZhMDg1ZjIyYjQ2NTVmNDhjZDVhMzZhOTIxNWY2NGMwNjI4MzgiLCJwaWN0dXJlIjoiaHR0cDovL2NhbnZhcy5pbnN0cnVjdHVyZS5jb20vaW1hZ2VzL21lc3NhZ2VzL2F2YXRhci01MC5wbmciLCJlbWFpbCI6ImRtY2NhbGx1bUB1bmljb24ubmV0IiwibmFtZSI6ImRtY2NhbGx1bUB1bmljb24ubmV0IiwiZ2l2ZW5fbmFtZSI6ImRtY2NhbGx1bUB1bmljb24ubmV0IiwiZmFtaWx5X25hbWUiOiIiLCJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9saXMiOnsicGVyc29uX3NvdXJjZWRpZCI6bnVsbCwiY291cnNlX29mZmVyaW5nX3NvdXJjZWRpZCI6bnVsbH0sImxvY2FsZSI6ImVuIiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vcm9sZXMiOlsiaHR0cDovL3B1cmwuaW1zZ2xvYmFsLm9yZy92b2NhYi9saXMvdjIvaW5zdGl0dXRpb24vcGVyc29uI0FkbWluaXN0cmF0b3IiXSwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vY29udGV4dCI6eyJpZCI6IjRkZGUwNWU4Y2ExOTczYmNjYTliZmZjMTNlMTU0ODgyMGVlZTkzYTMiLCJsYWJlbCI6IkRNIFRlc3QgMSIsInRpdGxlIjoiRE0gVGVzdCAxIiwidHlwZSI6WyJodHRwOi8vcHVybC5pbXNnbG9iYWwub3JnL3ZvY2FiL2xpcy92Mi9jb3Vyc2UjQ291cnNlT2ZmZXJpbmciXX0sImh0dHBzOi8vcHVybC5pbXNnbG9iYWwub3JnL3NwZWMvbHRpL2NsYWltL3Rvb2xfcGxhdGZvcm0iOnsiZ3VpZCI6Ik9PTjh0ZnI1QUJkNEVLYmFXYmhsaGl1TUNKMENlSVFTVU42aWtWQTU6Y2FudmFzLWxtcyIsIm5hbWUiOiJMVEkgQWR2YW50YWdlIiwidmVyc2lvbiI6ImNsb3VkIiwicHJvZHVjdF9mYW1pbHlfY29kZSI6ImNhbnZhcyJ9LCJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9sYXVuY2hfcHJlc2VudGF0aW9uIjp7ImRvY3VtZW50X3RhcmdldCI6ImlmcmFtZSIsImhlaWdodCI6NDAwLCJ3aWR0aCI6ODAwLCJyZXR1cm5fdXJsIjoiaHR0cDovL2NhbnZhcy5kb2NrZXIvY291cnNlcy8xL2V4dGVybmFsX2NvbnRlbnQvc3VjY2Vzcy9leHRlcm5hbF90b29sX3JlZGlyZWN0IiwibG9jYWxlIjoiZW4ifSwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vY3VzdG9tIjp7fSwiaHR0cHM6Ly93d3cuaW5zdHJ1Y3R1cmUuY29tL2NhbnZhc191c2VyX2lkIjoxLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vY2FudmFzX3VzZXJfbG9naW5faWQiOiJkbWNjYWxsdW1AdW5pY29uLm5ldCIsImh0dHBzOi8vd3d3Lmluc3RydWN0dXJlLmNvbS9jYW52YXNfYXBpX2RvbWFpbiI6ImNhbnZhcy5kb2NrZXIiLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vY2FudmFzX2NvdXJzZV9pZCI6MSwiaHR0cHM6Ly93d3cuaW5zdHJ1Y3R1cmUuY29tL2NhbnZhc193b3JrZmxvd19zdGF0ZSI6ImNsYWltZWQiLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vbGlzX2NvdXJzZV9vZmZlcmluZ19zb3VyY2VkaWQiOm51bGwsImh0dHBzOi8vd3d3Lmluc3RydWN0dXJlLmNvbS9yb2xlcyI6InVybjpsdGk6aW5zdHJvbGU6aW1zL2xpcy9BZG1pbmlzdHJhdG9yLHVybjpsdGk6c3lzcm9sZTppbXMvbGlzL1N5c0FkbWluLHVybjpsdGk6c3lzcm9sZTppbXMvbGlzL1VzZXIiLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vY2FudmFzX2Vucm9sbG1lbnRfc3RhdGUiOiIifQ.c9Qay0rCJy0zE9Td2_jSkN0NqueC0hdFhr50lyc_NombVFzPV4dFEVpiCn_8BGB9yDcAQedXj-KBf6uvcgdmjA";

            data.SignedEncodedToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IjIwMTgtMDYtMThUMjI6MzM6MjBaIn0.eyJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9tZXNzYWdlX3R5cGUiOiJMdGlSZXNvdXJjZUxpbmtSZXF1ZXN0IiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vdmVyc2lvbiI6IjEuMy4wIiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vcmVzb3VyY2VfbGluayI6eyJpZCI6IjRkZGUwNWU4Y2ExOTczYmNjYTliZmZjMTNlMTU0ODgyMGVlZTkzYTMifSwiYXVkIjoxMDAwMDAwMDAwMDAwMSwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vZGVwbG95bWVudF9pZCI6IjQ6NGRkZTA1ZThjYTE5NzNiY2NhOWJmZmMxM2UxNTQ4ODIwZWVlOTNhMyIsImV4cCI6MTUzNzIxNDUwNSwiaWF0IjoxNTM3MjEwOTA1LCJpc3MiOiJodHRwczovL2NhbnZhcy5pbnN0cnVjdHVyZS5jb20iLCJub25jZSI6IjRkNWQzM2JjLTYwZWItNGNjZS1hMGExLTA5MjgwMzE1NTM3NCIsInN1YiI6IjUzNWZhMDg1ZjIyYjQ2NTVmNDhjZDVhMzZhOTIxNWY2NGMwNjI4MzgiLCJwaWN0dXJlIjoiaHR0cDovL2NhbnZhcy5pbnN0cnVjdHVyZS5jb20vaW1hZ2VzL21lc3NhZ2VzL2F2YXRhci01MC5wbmciLCJlbWFpbCI6ImRtY2NhbGx1bUB1bmljb24ubmV0IiwibmFtZSI6ImRtY2NhbGx1bUB1bmljb24ubmV0IiwiZ2l2ZW5fbmFtZSI6ImRtY2NhbGx1bUB1bmljb24ubmV0IiwiZmFtaWx5X25hbWUiOiIiLCJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9saXMiOnsicGVyc29uX3NvdXJjZWRpZCI6bnVsbCwiY291cnNlX29mZmVyaW5nX3NvdXJjZWRpZCI6bnVsbH0sImxvY2FsZSI6ImVuIiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vcm9sZXMiOlsiaHR0cDovL3B1cmwuaW1zZ2xvYmFsLm9yZy92b2NhYi9saXMvdjIvaW5zdGl0dXRpb24vcGVyc29uI0FkbWluaXN0cmF0b3IiXSwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vY29udGV4dCI6eyJpZCI6IjRkZGUwNWU4Y2ExOTczYmNjYTliZmZjMTNlMTU0ODgyMGVlZTkzYTMiLCJsYWJlbCI6IkRNIFRlc3QgMSIsInRpdGxlIjoiRE0gVGVzdCAxIiwidHlwZSI6WyJodHRwOi8vcHVybC5pbXNnbG9iYWwub3JnL3ZvY2FiL2xpcy92Mi9jb3Vyc2UjQ291cnNlT2ZmZXJpbmciXX0sImh0dHBzOi8vcHVybC5pbXNnbG9iYWwub3JnL3NwZWMvbHRpL2NsYWltL3Rvb2xfcGxhdGZvcm0iOnsiZ3VpZCI6Ik9PTjh0ZnI1QUJkNEVLYmFXYmhsaGl1TUNKMENlSVFTVU42aWtWQTU6Y2FudmFzLWxtcyIsIm5hbWUiOiJMVEkgQWR2YW50YWdlIiwidmVyc2lvbiI6ImNsb3VkIiwicHJvZHVjdF9mYW1pbHlfY29kZSI6ImNhbnZhcyJ9LCJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9sYXVuY2hfcHJlc2VudGF0aW9uIjp7ImRvY3VtZW50X3RhcmdldCI6ImlmcmFtZSIsImhlaWdodCI6NDAwLCJ3aWR0aCI6ODAwLCJyZXR1cm5fdXJsIjoiaHR0cDovL2NhbnZhcy5kb2NrZXIvY291cnNlcy8xL2V4dGVybmFsX2NvbnRlbnQvc3VjY2Vzcy9leHRlcm5hbF90b29sX3JlZGlyZWN0IiwibG9jYWxlIjoiZW4ifSwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vY3VzdG9tIjp7fSwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGktbnJwcy9jbGFpbS9uYW1lc3JvbGVzZXJ2aWNlIjp7ImNvbnRleHRfbWVtYmVyc2hpcHNfdXJsIjoiaHR0cDovL2NhbnZhcy5kb2NrZXIvYXBpL2x0aS9jb3Vyc2VzLzEvbWVtYmVyc2hpcF9zZXJ2aWNlX3YyIiwic2VydmljZV92ZXJzaW9uIjoiMi4wIn0sImh0dHBzOi8vd3d3Lmluc3RydWN0dXJlLmNvbS9jYW52YXNfdXNlcl9pZCI6MSwiaHR0cHM6Ly93d3cuaW5zdHJ1Y3R1cmUuY29tL2NhbnZhc191c2VyX2xvZ2luX2lkIjoiZG1jY2FsbHVtQHVuaWNvbi5uZXQiLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vY2FudmFzX2FwaV9kb21haW4iOiJjYW52YXMuZG9ja2VyIiwiaHR0cHM6Ly93d3cuaW5zdHJ1Y3R1cmUuY29tL2NhbnZhc19jb3Vyc2VfaWQiOjEsImh0dHBzOi8vd3d3Lmluc3RydWN0dXJlLmNvbS9jYW52YXNfd29ya2Zsb3dfc3RhdGUiOiJhdmFpbGFibGUiLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vbGlzX2NvdXJzZV9vZmZlcmluZ19zb3VyY2VkaWQiOm51bGwsImh0dHBzOi8vd3d3Lmluc3RydWN0dXJlLmNvbS9yb2xlcyI6InVybjpsdGk6aW5zdHJvbGU6aW1zL2xpcy9BZG1pbmlzdHJhdG9yLHVybjpsdGk6c3lzcm9sZTppbXMvbGlzL1N5c0FkbWluLHVybjpsdGk6c3lzcm9sZTppbXMvbGlzL1VzZXIiLCJodHRwczovL3d3dy5pbnN0cnVjdHVyZS5jb20vY2FudmFzX2Vucm9sbG1lbnRfc3RhdGUiOiIifQ.Ynvu3k07NLO5smXccUX_xFkqQEC0HYSWkaUbkyNfUzBKXuYEO4b5-2bFZfu1RcSo_o-E_bHOCyKCUutYekp-Og";
            data.Token = _tokenHandler.ReadToken(data.SignedEncodedToken);
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

            return data;
        }

        static DemoData CreateLtiLaunchAsymmetricKeyData()
        {
            /*
                Step 0: Set up an IMS Reference Tool

                Name: PSU LTI1.3DotNet
                Client: 094538B8-3E5F-4713-8EAC-B14B8FCDF9BE
                Private key: ...
                Deployment: https://localhost/Default.aspx
                Keyset url:
                oauth2 url:
            */

            DemoData data = new DemoData { Name = "LTI Launch JWT Token From LMS to LTI Asymmetric Key" };

            /* 
                Step 1: Found a Tool Launch for the tool in IMS Ref Tool (from User: dmccallum@unicon.net).
            */

            // raw JWT containing all LTI launch data. This would come from Canvas
            data.SignedEncodedToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IlZhNWFpcUtlWHZBMTlaRTlTYjl3clJVcVEwbHpnRFVCN2wyWjM1RVR3Yk0ifQ.eyJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9tZXNzYWdlX3R5cGUiOiJMdGlEZWVwTGlua2luZ1JlcXVlc3QiLCJnaXZlbl9uYW1lIjoiTGFuaSIsImZhbWlseV9uYW1lIjoiV2F0c2ljYSIsIm1pZGRsZV9uYW1lIjoiQXNoYSIsInBpY3R1cmUiOiJodHRwOi8vZXhhbXBsZS5vcmcvTGFuaS5qcGciLCJlbWFpbCI6IkxhbmkuV2F0c2ljYUBleGFtcGxlLm9yZyIsIm5hbWUiOiJMYW5pIEFzaGEgV2F0c2ljYSIsImh0dHBzOi8vcHVybC5pbXNnbG9iYWwub3JnL3NwZWMvbHRpL2NsYWltL3JvbGVzIjpbImh0dHA6Ly9wdXJsLmltc2dsb2JhbC5vcmcvdm9jYWIvbGlzL3YyL2luc3RpdHV0aW9uL3BlcnNvbiNJbnN0cnVjdG9yIl0sImh0dHBzOi8vcHVybC5pbXNnbG9iYWwub3JnL3NwZWMvbHRpL2NsYWltL3JvbGVfc2NvcGVfbWVudG9yIjpbImh0dHA6Ly9wdXJsLmltc2dsb2JhbC5vcmcvdm9jYWIvbGlzL3YyL2luc3RpdHV0aW9uL3BlcnNvbiNBZG1pbmlzdHJhdG9yIl0sImh0dHBzOi8vcHVybC5pbXNnbG9iYWwub3JnL3NwZWMvbHRpL2NsYWltL2NvbnRleHQiOnsiaWQiOiIyNSIsImxhYmVsIjoiZW5nXzEwMSIsInRpdGxlIjoiRW5nIDEwMSIsInR5cGUiOlsiQ291cnNlT2ZmZXJpbmciXX0sImh0dHBzOi8vcHVybC5pbXNnbG9iYWwub3JnL3NwZWMvbHRpL2NsYWltL3Rvb2xfcGxhdGZvcm0iOnsibmFtZSI6IkphbWVzIFRlc3QgUGxhdGZvcm0iLCJjb250YWN0X2VtYWlsIjoiIiwiZGVzY3JpcHRpb24iOiIiLCJ1cmwiOiIiLCJwcm9kdWN0X2ZhbWlseV9jb2RlIjoiIiwidmVyc2lvbiI6IjEuMCJ9LCJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS1kbC9jbGFpbS9kZWVwX2xpbmtpbmdfc2V0dGluZ3MiOnsiYWNjZXB0X3R5cGVzIjpbImxpbmsiLCJmaWxlIiwiaHRtbCIsImx0aUxpbmsiLCJpbWFnZSJdLCJhY2NlcHRfbWVkaWFfdHlwZXMiOiJpbWFnZS8qLHRleHQvaHRtbCIsImFjY2VwdF9wcmVzZW50YXRpb25fZG9jdW1lbnRfdGFyZ2V0cyI6WyJpZnJhbWUiLCJ3aW5kb3ciLCJlbWJlZCJdLCJhY2NlcHRfbXVsdGlwbGUiOnRydWUsImF1dG9fY3JlYXRlIjp0cnVlLCJ0aXRsZSI6IlRoaXMgaXMgdGhlIGRlZmF1bHQgdGl0bGUiLCJ0ZXh0IjoiVGhpcyBpcyB0aGUgZGVmYXVsdCB0ZXh0IiwiZGF0YSI6IlNvbWUgcmFuZG9tIG9wYXF1ZSBkYXRhIHRoYXQgTVVTVCBiZSBzZW50IGJhY2siLCJkZWVwX2xpbmtfcmV0dXJuX3VybCI6Imh0dHBzOi8vbHRpLXJpLmltc2dsb2JhbC5vcmcvcGxhdGZvcm1zLzI3L2NvbnRleHRzLzI1L2RlZXBfbGlua3MifSwiaXNzIjoiaHR0cHM6Ly9sdGktcmkuaW1zZ2xvYmFsLm9yZyIsImF1ZCI6IjEyMzQ1IiwiaWF0IjoxNTM3MTk1ODYyLCJleHAiOjE1MzcxOTYxNjIsInN1YiI6ImViMGZmNDQ1MDhiOTNjMzExN2FlIiwibm9uY2UiOiI4OGFmOTgwODg2Y2E4MzE4Y2MxOCIsImh0dHBzOi8vcHVybC5pbXNnbG9iYWwub3JnL3NwZWMvbHRpL2NsYWltL3ZlcnNpb24iOiIxLjMuMCIsImxvY2FsZSI6ImVuLVVTIiwiaHR0cHM6Ly9wdXJsLmltc2dsb2JhbC5vcmcvc3BlYy9sdGkvY2xhaW0vbGF1bmNoX3ByZXNlbnRhdGlvbiI6eyJkb2N1bWVudF90YXJnZXQiOiJpZnJhbWUiLCJoZWlnaHQiOjMyMCwid2lkdGgiOjI0MH0sImh0dHBzOi8vd3d3LmV4YW1wbGUuY29tL2V4dGVuc2lvbiI6eyJjb2xvciI6InZpb2xldCJ9LCJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9jdXN0b20iOnsibXlDdXN0b21WYWx1ZSI6IjEyMyJ9LCJodHRwczovL3B1cmwuaW1zZ2xvYmFsLm9yZy9zcGVjL2x0aS9jbGFpbS9kZXBsb3ltZW50X2lkIjoiMTIzIn0.J4QZbIbxEz2QVXgAIqyqS-hQsPZqX3WWeNVGtSpneYdPHSxe28s2kUI4_fkBzJ6KWu1gDodkXMHc5Yh_P5236IjNbz_ZiFcftzlOgKG-jBmD5x5YWsKksXYKLLRik5iLiFX_UKmCOxNINpqC6BMES4k2QqbstqTYLbyNzco_1vEJlaCyROCHU9U2fD6MY90yaxfGDil_EujOrXZva6H8mNhS5DrzLvBHaJIBOELjkytVGjGMA9_Bkn6b368HGq1viI-S5MJY4JQTIOj9IJYuKGWy_XPit7yWKl-JciZnaeF3OwQsDU7htdZXuwwl_mRu0bNA_kKS-mIQuP6ZFEQcdw";

            /*
                Step 2: Tool validates the token
            */

            // signing key generated using, the public key from the tool set up in Step 0
            data.SigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("???"));

            // validation parameters, using values under Security Details
            data.ValidationParameters = new TokenValidationParameters()
            {
                ValidAudiences = new string[]
                {
                    "???" // "aud": 10000000000001
                },
                ValidIssuers = new string[]
                {
                    "???" // "iss"
                },
                IssuerSigningKey = data.SigningKey
            };

            // signing credentials, using the algorithm corresponding to "alg": ""RS256" in the launch
            data.SigningCredentials = new SigningCredentials(data.SigningKey, SecurityAlgorithms.RsaSha256, SecurityAlgorithms.RsaSha256Signature);

            data.Validate(_tokenHandler);

            return data;
        }
    }

    public static class Extensions
    {
        public static string Val(this JwtPayload payload, string key)
        {
            if (payload.ContainsKey(key))
                return payload[key].ToString();

            return "";
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
        public bool Valid { get; set; }
        public string ValidationMessage { get; set; }
        public void Validate(SecurityTokenHandler _tokenHandler)
        {
            if (string.IsNullOrWhiteSpace(SignedEncodedToken) || ValidationParameters == null)
            {
                Valid = false;
                ValidationMessage = "Missing parameters to Validate() method";
                return;
            }

            //Token = _tokenHandler.ReadToken(SignedEncodedToken);

            try
            {
                _tokenHandler.ValidateToken(SignedEncodedToken, ValidationParameters, out SecurityToken validatedToken);

                Token = validatedToken;
                Valid = true;
                ValidationMessage = "Is Valid";
            }
            catch (Exception e) {
                Valid = false;
                ValidationMessage = e.Message;
            }
        }

        public void Dump()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("-------------------------------------");
            sb.AppendLine(Name);
            sb.AppendLine("-------------------------------------");

            //sb.AppendLine($"{JsonConvert.SerializeObject(Token, Formatting.Indented)}");
            //sb.AppendLine();

            var jwtToken = (JwtSecurityToken)Token;
            var payload = jwtToken.Payload;
            
            sb.AppendLine($"LTI Version      : {payload.Val("https://purl.imsglobal.org/spec/lti/claim/version")}");
            // version detection? For LTI1.1 the version will be in a Form field, and for 1.3 we'll need to unpack the token...or something

            sb.AppendLine($"Name             : {payload.Val("name")}");
            sb.AppendLine($"Valid            : {ValidationMessage}");
            sb.AppendLine($"Email            : {payload.Val("email")}");
            sb.AppendLine($"Canvas Login Id  : {payload.Val("https://www.instructure.com/canvas_user_login_id")}");
            sb.AppendLine($"Canvas Course Id : {payload.Val("https://www.instructure.com/canvas_course_id")}");

            int iatEpoch = Convert.ToInt32(payload.Val("iat"));
            int expEpoch = Convert.ToInt32(payload.Val("exp"));
            TimeSpan iat = new TimeSpan(0, 0, iatEpoch);
            TimeSpan exp = new TimeSpan(0, 0, expEpoch);
            DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0);
            sb.AppendLine($"Issued at        : {baseDate.Add(iat)}");
            sb.AppendLine($"Expires at       : {baseDate.Add(exp)}");

            // IMS defined roles - value is a JSON array
            sb.AppendLine().AppendLine("IMS Claim Roles (https://purl.imsglobal.org/spec/lti/claim/roles)");
            string imsRolesJson = payload.Val("https://purl.imsglobal.org/spec/lti/claim/roles").ToString();
            string[] imsRoles = JsonConvert.DeserializeObject<string[]>(imsRolesJson);
            foreach (string imsRole in imsRoles)
            {
                sb.AppendLine($"  {imsRole}");
            }

            // Canvas specific roles
            sb.AppendLine().AppendLine("Canvas Non-Claim Roles (https://www.instructure.com/roles)");
            string canvasRolesCsv = payload.Val("https://www.instructure.com/roles").ToString();
            string[] canvasRoles = canvasRolesCsv.Split(',');
            foreach (string canvasRole in canvasRoles)
            {
                sb.AppendLine($"  {canvasRole}");
            }

            // hopefully these Claim keys are the same as with the Form based LTI Requests

            Console.WriteLine(sb.ToString());
        }
    }

}
