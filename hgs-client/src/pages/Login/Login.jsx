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
import { useState } from "react";
import { useForm } from "react-hook-form";
import * as z from "zod";
import "./Login.scss";
import { useLoginMutation } from "@/services/common/mutation";
// import schoolLogo from "@/assets/images/school-logo.png"; // Add a school logo to your assets

const formSchema = z.object({
  username: z.string().min(1, "Tên đăng nhập là bắt buộc"),
  password: z.string().min(6, "Mật khẩu phải có ít nhất 6 ký tự"),
});

const Login = () => {
  const loginMutation = useLoginMutation();

  const form = useForm({
    resolver: zodResolver(formSchema),
    defaultValues: {
      username: "",
      password: "",
    },
  });

  const onSubmit = async (values) => {
    loginMutation.mutate(values);
  };

  return (
    <div className="login-container">
      <div className="login-content">
        <div className="login-left">
          <div className="login-welcome">
            <h1>Chào mừng đến với</h1>
            <p className="school-name">Trường THCS Hải Giang</p>
          </div>
        </div>

        <Card className="login-card">
          <CardHeader className="space-y-1">
            <div className="mb-4 flex justify-center">
              <div className="logo-container">
                <span className="logo-text">HGS</span>
              </div>
            </div>
            <CardTitle className="text-center text-2xl font-bold">
              Trường THCS Hải Giang
            </CardTitle>
            <CardDescription className="text-center">
              Đăng nhập để truy cập vào hệ thống
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Form {...form}>
              <form
                onSubmit={form.handleSubmit(onSubmit)}
                className="space-y-4"
              >
                <FormField
                  control={form.control}
                  name="username"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Tên đăng nhập</FormLabel>
                      <FormControl>
                        <div className="relative">
                          <Input
                            placeholder="Nhập tên đăng nhập..."
                            className="pl-10"
                            {...field}
                          />
                        </div>
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="password"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Mật khẩu</FormLabel>
                      <FormControl>
                        <div className="relative">
                          <Input
                            type="password"
                            placeholder="Nhập mật khẩu..."
                            className="pl-10"
                            {...field}
                          />
                        </div>
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <Button
                  type="submit"
                  className="login-button w-full"
                  disabled={loginMutation.isLoading}
                >
                  {loginMutation.isLoading ? "Đang đăng nhập..." : "Đăng nhập"}
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
