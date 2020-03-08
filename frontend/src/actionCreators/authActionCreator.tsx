import { showLoading, hideLoading } from "react-redux-loading-bar";
import { request, success, error } from "actions/genericActions";
import * as reducerTypes from "constants/reducerTypes";
import * as API from "constants/API";
import { ENVIRONMENT } from "constants/environment";
import CustomAxios from "customAxios";
import { AxiosResponse } from "axios";
import { createRequestHeader } from "utils/RequestHeaders";

export const getActivateUserAction = () => (dispatch: Function) => {
    dispatch(request(reducerTypes.POST_ACTIVATE_USER));
    dispatch(showLoading());
    return CustomAxios()
        .post(
            ENVIRONMENT.apiUrl + API.ACTIVATE_USER(),
            null,
            createRequestHeader()
        )
        .then((response: AxiosResponse) => {
            dispatch(success(reducerTypes.POST_ACTIVATE_USER, response.status));
            dispatch(hideLoading());
        })
        .catch(() => dispatch(error(reducerTypes.POST_ACTIVATE_USER)))
        .finally(() => dispatch(hideLoading()));
};