import React, { useState } from 'react';
import UploadLessonPlan from './UploadLessonPlan.jsx';
import LessonPlanList from './LessonPlanList.jsx';
import ReviewList from './ReviewList/ReviewList.jsx';

const RequestLessonPlan = () => {
    const [activeTab, setActiveTab] = useState('upload'); // Mặc định hiển thị form upload

    const renderContent = () => {
        switch (activeTab) {
            case 'upload':
                return <UploadLessonPlan />;
            case 'list':
                return <LessonPlanList />;
            case 'review':
                return <ReviewList />;

            default:
                return <UploadLessonPlan />;
        }
    };

    return (
        <div className="p-6">
            {/* Bộ lọc */}
            <div className="mb-4 flex space-x-4">
                <button
                    onClick={() => setActiveTab('upload')}
                    className={`py-2 px-4 rounded text-black transition-colors border
                    ${activeTab === 'upload' ? 'border-gray-500 bg-gray-100' : 'border-gray-300 hover:bg-gray-100'}`}
                >
                    Tạo Kế Hoạch Mới
                </button>

                <button
                    onClick={() => setActiveTab('list')}
                    className={`py-2 px-4 rounded text-black transition-colors border
                    ${activeTab === 'list' ? 'border-gray-500 bg-gray-100' : 'border-gray-300 hover:bg-gray-100'}`}
                >
                    Xem Danh Sách
                </button>

                <button
                    onClick={() => setActiveTab('review')}
                    className={`py-2 px-4 rounded text-black transition-colors border
                    ${activeTab === 'review' ? 'border-gray-500 bg-gray-100' : 'border-gray-300 hover:bg-gray-100'}`}
                >
                    Duyệt Kế Hoạch
                </button>
            </div>


            {/* Main content */}
            <div>
                {renderContent()}
            </div>
        </div>
    );
};

export default RequestLessonPlan;
