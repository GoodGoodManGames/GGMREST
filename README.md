# GGMRESTFul Client
GGMRESTFul은 Android진영의 강력한 RESTFul 라이브러리인 Retrofit에서 영감을 받아
만들었습니다.

GGMRESTFul는 **GGMContext에 의존적이지 않으며** GGMBot에서 사용될 목적으로 개발되었습니다.
.net core 2에서 구현되었으므로 기타 다 플랫폼과의 호환성을 가지고 있으나, Reflaction.Emit을 사용하여 동적으로 코드를 생성하기 떄문에 IOS같은 정적 컴파일 환경에선 동작하지 않습니다.


## 작동 원리
 내부적으로 Dynamic Proxy를 IL코드로 구현하여 Service(interface)에 선언된 메소드 사용시 해당 메소드의 정보들을 가져와 작업을 처리합니다. 이후 메소드와 파라매터에 적용된 Attribute들을 이용하여 Request 헤더와 보디를 만들어 전송하고, 이를 Json.NET을 이용하여 Deserialize 하여 반환합니다.

## 종속성
 해당 라이브러리는 [NewtonSoft 의 Json.NET](http://www.newtonsoft.com/json)을 내부적으로 사용합니다.

## 기능
* HTTP Method(GET/POST/PUT/DELETE) 지원
* HeaderAttribute 지원
* 동적인 API를 위한 PathAttribute, QueryAttribute 지원
* Request body전송을 위한 BodyAttribute지원
* 비동기처리(더이상 동기적 데이터 요청은 지원하지 않음.)

## 사용 방법과 예제

**프로젝트 내의 UnitTest 프로젝트를 참고하세요.**

아래의 예제는 [JSONPlaceholder](https://jsonplaceholder.typicode.com/) 서버를 이용한 예제입니다.
반환되는 JSON데이터를 저장받을 모델 클래스를 정의합니다.
```cs
public class TestPost
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }

    public TestPost(string title, string body, int id = 0)
    {
        Id = id;
        Title = title;
        Body = body;
    }

    public override string ToString()
    {
        return string.Concat("UserID : ", Id, ", Title : ", Title, ", Body", Body);
    }
}
```


 API를 호출할 Service 인터페이스를 선언합니다. 모든 메소드에는 HTTPMethodAttribute가 반드시 지정되어 있어야 합니다.
```cs
public interface ITestService
{
    [GET("posts/{id}")] // 메소드의 파라메터 중 [Path] Attribute 을 이용하면 동적으로 api 을 정할 수 있습니다.
    TestPost GetPost([Path("id")] int postId);

    [GET("posts")]
    List<TestPost> GetPosts();

    [Header("Content-type", "application/json")] // HeaderAttribute 을 이용해 Header 을 정할 수 도 있습니다.

    [POST("posts")]
    TestPost CreatePost([Body]TestPost post);


    [Header("Content-type", "application/json")]
    [PUT("posts/{id}")]
    TestPost RewritePost([Path("id")] int postId, [Body]TestPost post);

    [DELETE("posts/{id}")]
    TestPost DeletePost([Path("id")] int postId);

}
```

Service생성을 위해, 우선 Factory객체를 생성합니다. SetDefaultHeaders를 이용하여 기본적인 헤더들을 미리 정해 놓을 수 있습니다.
생성된 service를 이용하여 RESTFul클라이언트를 테스트 해 볼 수 있습니다.

```cs
public static void Main(string[] args)
{
    Factory factory = new Factory("https://jsonplaceholder.typicode.com/");
    ITestService testService = factory.Create<ITestService>();
    Console.WriteLine(testService.GetPost(1).ToString()+"\n");
    Console.WriteLine(testService.GetPosts().ToString() + "\n");
    Console.WriteLine(testService.CreatePost(new TestPost("TestPost", "TestBodt")).ToString() + "\n");
    Console.WriteLine(testService.RewritePost(1, new TestPost("RewritePost", "RewriteBody")).ToString() + "\n");
    Console.WriteLine(testService.DeletePost(1).ToString() + "\n");

}
```

## 추후 구현 기능 및 예정
* 추상화된 JsonConverter, RequestBuilder를 구현 (사용자가 다른 Json라이브러리나 Request을 이용할 수 있게끔
* 예외 구현, Http response code를 확인할 수 있게끔 구현
* .NET 하위버전 호환


## 참조
 해당 프로젝트의 Dynamic Proxy는 [Dynamic Interface Implementation](https://www.codeproject.com/KB/dotnet/742788/DynamicProxyImplementation.zip)의 내용을 참조하였습니다.
## License
 해당 라이브러리는 MIT라이선스를 사용합니다, 코드의 공개를 할 필요 없으며 license 파일 사본만 포함시키면 됩니다.
  Dynamic Proxy 부분은 CPOL라이선스를 사용합니다.
