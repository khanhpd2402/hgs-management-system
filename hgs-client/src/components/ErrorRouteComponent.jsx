import { Link, isRouteErrorResponse, useRouteError } from "react-router";
import GradientText from "./GradientText";

const ErrorRouteComponent = () => {
  const error = useRouteError();
  console.log(isRouteErrorResponse(error));

  return (
    <div className="font-montserrat flex h-screen w-screen items-center bg-gray-50">
      <div className="container flex flex-col items-center justify-between px-5 text-gray-700 md:flex-row">
        <div className="mx-8 w-full lg:w-1/2">
          <GradientText content="404" />
          <p className="mb-8 text-2xl leading-normal font-light md:text-3xl">
            Sorry we couldn't find the page you're looking for
          </p>
          <div
            id="error-page"
            className="flex flex-col items-center justify-center gap-8"
          >
            <p className="text-slate-400">
              <i>
                {isRouteErrorResponse(error)
                  ? error.error?.message || error.statusText || error.message
                  : "Unknown error message"}
              </i>
            </p>
            <Link to={"/"} className="underline">
              Return Home
            </Link>
          </div>
        </div>
        <div className="mx-5 my-12 w-full lg:flex lg:w-1/2 lg:justify-end">
          <img
            src="https://user-images.githubusercontent.com/43953425/166269493-acd08ccb-4df3-4474-95c7-ad1034d3c070.svg"
            className=""
            alt="Page not found"
          />
        </div>
      </div>
    </div>
  );
};

export default ErrorRouteComponent;
