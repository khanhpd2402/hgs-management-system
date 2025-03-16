import { Card, CardContent } from "@/components/ui/card";
import { Avatar } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";

export default function StudentProfile() {
  return (
    <div className="grid grid-cols-2 gap-4 p-6 md:grid-cols-2">
      {/* Thông tin học sinh */}

      {/* Cột bên trái: Thông tin cha & mẹ */}
      <div className="flex flex-col gap-4">
        <Card className="col-span-2 flex items-center p-4">
          <Avatar className="mr-4 h-24 w-24">
            <img
              src="https://plus.unsplash.com/premium_photo-1664536392779-049ba8fde933?q=80&w=1974&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D"
              alt="Avatar"
              className="h-full w-full rounded-full object-cover"
            />
          </Avatar>

          <div>
            <h2 className="text-lg font-semibold">Phạm Thị Vân Anh</h2>
            <p className="text-sm text-gray-600">05/06/2013 - Nữ</p>
            <p className="text-sm text-gray-600">Khối 6 - Lớp 6A</p>
            <Badge className="bg-green-500 text-white">Đang học</Badge>
          </div>
        </Card>
        <Card>
          <CardContent className="p-4">
            <h3 className="font-semibold">Thông tin cha</h3>
            <p>
              <strong>Họ và tên:</strong> Phạm Văn Dương
            </p>
            <p>
              <strong>Nghề nghiệp:</strong> Làm ruộng
            </p>
            <p>
              <strong>SDT:</strong> 0858978186
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <h3 className="font-semibold">Thông tin mẹ</h3>
            <p>
              <strong>Họ và tên:</strong> Trần Thị Oanh
            </p>
            <p>
              <strong>Nghề nghiệp:</strong> Làm ruộng
            </p>
            <p>
              <strong>SDT:</strong> 0948968638
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Cột bên phải: Thông tin chung & Hộ khẩu */}
      <div className="flex flex-col gap-4">
        <Card>
          <CardContent className="p-4">
            <h3 className="font-semibold">Thông tin chung</h3>
            <p>Số đăng bộ: 4/2024/HG/THCS</p>
            <p>
              Hình thức trúng tuyển: <strong>Trúng tuyển</strong>
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <h3 className="font-semibold">Thông tin địa chỉ & Hộ khẩu</h3>
            <p>
              <strong>Tỉnh/TP thường trú:</strong> Tỉnh Nam Định
            </p>
            <p>
              <strong>Xã/phường thường trú:</strong> Xã Hải Giang
            </p>
            <p>
              <strong>Thôn/xóm thường trú:</strong> lorem20
            </p>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
