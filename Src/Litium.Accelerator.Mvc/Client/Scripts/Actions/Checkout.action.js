import { post, put, remove } from '../Services/http';
import { load as loadCart } from './Cart.action';
import { catchError } from './Error.action';

import {
    CHECKOUT_SET_SELECTED_COMPANY_ADDRESS,
    CHECKOUT_SET_CUSTOMER_INFO,
    CHECKOUT_SET_PRIVATE_CUSTOMER,
    CHECKOUT_SET_SIGN_UP,
    CHECKOUT_SET_DELIVERY,
    CHECKOUT_SET_COUNTRY,
    CHECKOUT_SET_PAYMENT,
    CHECKOUT_SET_DISCOUNT_CODE,
    CHECKOUT_SET_ORDER_NOTE,
    CHECKOUT_ACCEPT_TERMS_OF_CONDITION,
    CHECKOUT_SUBMIT,
    CHECKOUT_SUBMIT_ERROR,
    CHECKOUT_SET_PAYMENT_WIDGET,
    CHECKOUT_SET_USED_DISCOUNT_CODE,
} from '../constants';

export const setBusinessCustomer = (isBusinessCustomer) => ({
    type: CHECKOUT_SET_PRIVATE_CUSTOMER,
    payload: {
        isBusinessCustomer,
    },
});

export const setSignUp = (signUp) => ({
    type: CHECKOUT_SET_SIGN_UP,
    payload: {
        signUp,
    },
});

export const setAlternativeAddress = (propName, value) => ({
    type: CHECKOUT_SET_CUSTOMER_INFO,
    payload: {
        key: 'alternativeAddress',
        data: {
            [propName]: value,
        },
    },
});

export const setCustomerDetails = (propName, value) => ({
    type: CHECKOUT_SET_CUSTOMER_INFO,
    payload: {
        key: 'customerDetails',
        data: {
            [propName]: value,
        },
    },
});

export const setSelectedCompanyAddress = (selectedCompanyAddressId) => ({
    type: CHECKOUT_SET_SELECTED_COMPANY_ADDRESS,
    payload: {
        selectedCompanyAddressId,
    },
});

export const setDelivery = (systemId) => (dispatch, getState) => {
    dispatch({
        type: CHECKOUT_SET_DELIVERY,
        payload: {
            selectedDeliveryMethod: systemId,
        },
    });
    const { payload } = getState().checkout;
    return put('/api/checkout/setDeliveryProvider', payload)
        .then((response) => response.json())
        .then((result) => {
            dispatch(loadCart());
            if (
                result !== null &&
                result.paymentWidget &&
                result.paymentWidget.displayDeliveryMethods
            ) {
                dispatch(setPaymentWidget(result.paymentWidget));
            }
        })
        .catch((ex) => dispatch(catchError(ex, (error) => submitError(error))));
};

export const setCountry = (systemId) => (dispatch, getState) => {
    dispatch({
        type: CHECKOUT_SET_COUNTRY,
        payload: {
            selectedCountry: systemId,
        },
    });
    const { payload } = getState().checkout;
    return put('/api/checkout/setCountry', payload)
        .then((response) => response.json())
        .then((result) => {
            dispatch(loadCart());
            dispatch(setPaymentWidget(result.paymentWidget));
            dispatch({
                type: CHECKOUT_SET_DELIVERY,
                payload: {
                    deliveryMethods: result.deliveryMethods,
                    selectedDeliveryMethod: result.selectedDeliveryMethod,
                },
            });
            dispatch({
                type: CHECKOUT_SET_PAYMENT,
                payload: {
                    paymentMethods: result.paymentMethods,
                    selectedPaymentMethod: result.selectedPaymentMethod,
                },
            });
        })
        .catch((ex) => dispatch(catchError(ex, (error) => submitError(error))));
};

export const setPayment = (systemId) => (dispatch, getState) => {
    dispatch({
        type: CHECKOUT_SET_PAYMENT,
        payload: {
            selectedPaymentMethod: systemId,
        },
    });

    const { payload } = getState().checkout;
    const { paymentWidget } = payload;
    if (paymentWidget) {
        const paymentWidgetUpdated = {
            ...paymentWidget,
            hidePaymentWidget: true,
        };
        dispatch(setPaymentWidget(paymentWidgetUpdated));
    }
    // reset payment error on the checkout
    dispatch(
        submitError({
            modelState: {
                payment: [],
            },
        })
    );

    return put('/api/checkout/setPaymentProvider', payload)
        .then((response) => response.json())
        .then((result) => {
            dispatch(loadCart());
            dispatch(setPaymentWidget(result.paymentWidget));
        })
        .catch((ex) => dispatch(catchError(ex, (error) => submitError(error))));
};

export const reloadPayment = () => (dispatch, getState) => {
    const { payload } = getState().checkout;
    if (payload && payload.paymentWidget) {
        return put('/api/checkout/reloadPaymentWidget', payload)
            .then((response) => response.json())
            .then((result) => {
                dispatch(loadCart());
                if (
                    result &&
                    result.paymentWidget &&
                    result.paymentWidget.displayDeliveryMethods
                ) {
                    dispatch(setPaymentWidget(result.paymentWidget));
                }
            })
            .catch((ex) =>
                dispatch(catchError(ex, (error) => submitError(error)))
            );
    }
};

const setPaymentWidget = (paymentWidget) => ({
    type: CHECKOUT_SET_PAYMENT_WIDGET,
    payload: {
        paymentWidget,
    },
});

export const setOrderNote = (orderNote) => ({
    type: CHECKOUT_SET_ORDER_NOTE,
    payload: {
        orderNote,
    },
});

export const acceptTermsOfCondition = (acceptTermsOfCondition) => ({
    type: CHECKOUT_ACCEPT_TERMS_OF_CONDITION,
    payload: {
        acceptTermsOfCondition,
    },
});

export const setDiscountCode = (discountCode) => ({
    type: CHECKOUT_SET_DISCOUNT_CODE,
    payload: {
        discountCode,
    },
});

export const submitDiscountCode = () => (dispatch, getState) => {
    const { payload } = getState().checkout;
    return put('/api/checkout/setDiscountCode', payload)
        .then((response) => response.json())
        .then((result) => {
            dispatch(setUsedDiscountCodes(result.usedDiscountCodes));
            dispatch(loadCart());
            dispatch(setPaymentWidget(result.paymentWidget));
            // reset error of campaign code
            dispatch(
                submitError({
                    modelState: {
                        discountCode: [],
                    },
                })
            );
        })
        .catch((ex) => {
            dispatch(catchError(ex, (error) => submitError(error)));
            // restore the initial cart
            dispatch(loadCart());
        });
};

export const deleteDiscountCode = (discountCode) => (dispatch, getState) => {
    const { payload } = getState().checkout;
    payload.discountCode = discountCode;
    return remove('/api/checkout/deleteDiscountCode', payload)
        .then((response) => response.json())
        .then((result) => {
            dispatch(setUsedDiscountCodes(result.usedDiscountCodes));
            dispatch(loadCart());
            dispatch(setPaymentWidget(result.paymentWidget));
            dispatch(setDiscountCode(''));
            // reset error of campaign code
            dispatch(
                submitError({
                    modelState: {
                        discountCode: [],
                    },
                })
            );
        })
        .catch((ex) => {
            dispatch(catchError(ex, (error) => submitError(error)));
            // restore the initial cart
            dispatch(loadCart());
        });
};

const setUsedDiscountCodes = (usedDiscountCodes) => ({
    type: CHECKOUT_SET_USED_DISCOUNT_CODE,
    payload: {
        usedDiscountCodes,
    },
});

export const submit = () => (dispatch, getState) => {
    const { payload } = getState().checkout;
    return _submit('/api/checkout', payload, dispatch);
};

export const verify = (url, orderId, payload) => (dispatch, getState) => {
    const model = getState().checkout.payload;
    model.orderId = orderId;
    model.payload = payload;
    return _submit(url, model, dispatch);
};

const _submit = (url, model, dispatch) => {
    return post(url, model)
        .then((response) => response.json())
        .then((result) => {
            dispatch(submitDone(result));
        })
        .catch((ex) => {
            if (ex.response) {
                ex.response.json().then((error) => {
                    dispatch(submitError(error));
                    dispatch(submitDone(null));
                    // reload the cart, it might be changed after validation
                    dispatch(loadCart());
                });
            } else {
                dispatch(submitError(ex));
            }
        });
};

export const submitRequest = () => ({
    type: CHECKOUT_SUBMIT,
    payload: {
        isSubmitting: true,
        errors: [],
    },
});

export const submitDone = (result) => ({
    type: CHECKOUT_SUBMIT,
    payload: {
        result,
        isSubmitting: false,
    },
});

export const submitError = (error) => ({
    type: CHECKOUT_SUBMIT_ERROR,
    payload: {
        error,
    },
});
