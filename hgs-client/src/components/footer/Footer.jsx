import { Link } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Facebook, Youtube } from "lucide-react";

export default function Footer() {
  return (
    <footer className="mt-12 border-t bg-white py-10">
      <div className="container mx-auto grid grid-cols-1 gap-8 px-4 md:grid-cols-3">
        <div className="space-y-3">
          <div className="flex items-center gap-2">
            <img
              src="https://picsum.photos/200/300"
              alt="Logo trường THCS Hải Giang"
              className="h-10"
            />
            <div>
              <h3 className="text-lg font-semibold text-red-600">
                Trường THCS Hải Giang
              </h3>
            </div>
          </div>
          <p className="text-sm">
            <b>Địa chỉ:</b> Xã Hải Giang, Huyện Hải Hậu, Tỉnh Nam Định
          </p>
          <p className="text-sm">
            <b>Điện thoại:</b> 0123 456 789
          </p>
          <p className="text-sm">
            <b>Email:</b> thcshaigiang@edu.namdinh.gov.vn
          </p>
          <p className="text-sm">
            <b> &copy; {new Date().getFullYear()} Trường THCS Hải Giang</b>
          </p>
        </div>
      </div>
    </footer>
  );
}
