import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import * as z from "zod";
import "./Login.scss";
import { useLoginMutation } from "@/services/common/mutation";
import { useNavigate } from "react-router";
import { useEffect } from "react";
import { jwtDecode } from "jwt-decode";

const formSchema = z.object({
  username: z.string().min(1, "Tên đăng nhập là bắt buộc"),
  password: z.string().min(6, "Mật khẩu phải có ít nhất 6 ký tự"),
});

const Login = () => {
  const navigate = useNavigate();
  const loginMutation = useLoginMutation();

  // Check if user is already logged in
  useEffect(() => {
    const token = localStorage.getItem("token");
    const userRole = localStorage.getItem("userRole");

    if (token && userRole) {
      redirectBasedOnRole(userRole);
    }
  }, [navigate]);

  const redirectBasedOnRole = (role) => {
    switch (role) {
      case "Principal":
        navigate("/home");
        break;
      case "Teacher":
        navigate("/teacher/profile");
        break;
      default:
        navigate("/home");
    }
  };

  const form = useForm({
    resolver: zodResolver(formSchema),
    defaultValues: {
      username: "",
      password: "",
    },
  });

  const onSubmit = async (values) => {
    loginMutation.mutate(values, {
      onSuccess: (data) => {
        // Store token and user role in localStorage
        localStorage.setItem("token", JSON.stringify(data.token));
        const role = jwtDecode(data.token).role;

        // Redirect based on role
        redirectBasedOnRole(role);
      },
    });
  };
  return (
    <div className="login-container">
      <div className="login-content">
        <div className="login-left">
          <div className="login-welcome">
            <div className="school-logo">
              <span>HGS</span>
            </div>
            <h1>Chào mừng đến với</h1>
            <p className="school-name">Trường THCS Hải Giang</p>
            <p className="system-description">Hệ thống quản lý giáo dục</p>
          </div>
        </div>

        <Card className="login-card">
          <CardHeader className="space-y-1">
            <CardTitle className="text-center text-2xl font-bold">
              Đăng nhập hệ thống
            </CardTitle>
            <CardDescription className="text-center">
              Vui lòng nhập thông tin đăng nhập của bạn
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Form {...form}>
              <form
                onSubmit={form.handleSubmit(onSubmit)}
                className="space-y-6"
              >
                <FormField
                  control={form.control}
                  name="username"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel className="text-gray-700">
                        Tên đăng nhập
                      </FormLabel>
                      <FormControl>
                        <div className="relative">
                          <Input
                            placeholder="Nhập tên đăng nhập..."
                            className="border-gray-300 py-6 pr-4 pl-4 text-base focus-visible:ring-2 focus-visible:ring-blue-500"
                            {...field}
                          />
                        </div>
                      </FormControl>
                      <FormMessage className="text-sm text-red-500" />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="password"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel className="text-gray-700">Mật khẩu</FormLabel>
                      <FormControl>
                        <div className="relative">
                          <Input
                            type="password"
                            placeholder="Nhập mật khẩu..."
                            className="border-gray-300 py-6 pr-4 pl-4 text-base focus-visible:ring-2 focus-visible:ring-blue-500"
                            {...field}
                          />
                        </div>
                      </FormControl>
                      <FormMessage className="text-sm text-red-500" />
                    </FormItem>
                  )}
                />

                <Button
                  type="submit"
                  className="login-button w-full py-6 text-base font-medium"
                  disabled={loginMutation.isPending}
                >
                  {loginMutation.isPending ? "Đang đăng nhập..." : "Đăng nhập"}
                </Button>
              </form>
            </Form>
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

export default Login;
