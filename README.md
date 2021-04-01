# WdmServer
- TargetFramework: netcoreapp3.1

- [x] WDM 측정데이터 및 분석 데이터 관리 Web서버
- [x] PostgreSQL DB서버와 ASP.NET Core 기반이며 
- [x] 백엔드와 프린터엔드 모두에 C#을 사용가능한 Blazor 기술을 활용
- [x] 추후 추가되는 데이터모델을 런타임에 생성할 수 있는 Dynamic DB Context
- [x] 런타임 모델생성을 위해 `Universe.Web.Data.DynamicDbContext` 객체를 추가
- [x] 모든 데이터 모델은 `Universe.Web.Data.ModelBase<T>`를 상속
- [x] appsettings.json에서 모델이 구현된 어셈블리 파일 목록을 읽어와 모델을 생성

### 각 프로젝트, 폴더 용도
- `WebApp` : 실행 프로그램
- `CommonModels` : 기본적인 데이터 모델 구현
- `TagHelpers` : HTML 태그 확장
- `WebUtility` : 이메일, 쿠키 처리 등
- `Tester` : unit test project
- `Lib` : 참조된 어셈블리, 패키지 모음
- `Logger` : 로그 구현
- `ExecptionProcessing` : 예외처리 클래스
